using System;
using DG.Tweening;
using UnityEngine;

namespace Reversi.Stones.Stone
{
    /// <summary>
    /// ストーン アニメーション関連処理
    /// </summary>
    public partial class StoneBehaviour
    {
        [SerializeField] private GameObject putEffectPrefab;
        [NonSerialized] public bool IsDisplayAnimation;

        /// <summary>
        /// ストーンを置くエフェクト
        /// </summary>
        private void StartPutEffect()
        {
            // アニメーションさせない場合
            if (!IsDisplayAnimation) return;
            // パーティクルを生成
            var effect = Instantiate(putEffectPrefab, transform);
            effect.transform.localScale = putEffectPrefab.transform.localScale;
        }

        /// <summary>
        /// ひっくり返すアニメーション
        /// </summary>
        /// <param name="putStoneIndex"></param>
        /// <param name="callback"></param>
        private void StartTurnAnimation(StoneIndex putStoneIndex, Action callback)
        {
            // アニメーションさせない場合
            if (!IsDisplayAnimation)
            {
                callback();
                return;
            }
            // 色が変わる場合
            var putVec = Index - putStoneIndex;   // 置いた位置からのベクトル
            var waitTime = putVec.GetLength() * 0.08f; // アニメーションを遅らせる時間(遠いほど開始を遅らせる)
            var q = Quaternion.AngleAxis(180.0f, new Vector3(-putVec.Z, 0.0f, putVec.X)); // 90度回転させたクォータニオン(-Z,Xで指定)

            // ひっくり返るアニメーションを実行
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(waitTime);
            sequence.Append(stoneObject.transform.DOLocalRotateQuaternion(q, 0.5f).SetRelative().SetEase(Ease.OutQuart));
            sequence.Join(DOTween.Sequence()
                .Append(stoneObject.transform.DOLocalMoveY(1, 0.25f).SetRelative().SetEase(Ease.OutQuart))
                .Append(stoneObject.transform.DOLocalMoveY(-1, 0.2f).SetRelative().SetEase(Ease.InOutBounce)));
            sequence.OnComplete(() =>
            {
                callback();
            });
        }
    }
}
