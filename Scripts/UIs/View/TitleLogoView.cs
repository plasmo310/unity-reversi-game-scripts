using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Reversi.UIs.View
{
    /// <summary>
    /// タイトルロゴ
    /// </summary>
    public class TitleLogoView : MonoBehaviour
    {
        [SerializeField] private Image whiteBackImage;
        [SerializeField] private Image frontTextImage;
        [SerializeField] private Image backThunderImage;
        [SerializeField] private List<Image> backCircleImageList;
        private List<Tween> _loopTweenList;
        private static readonly float MinCircleImageScale = 0.95f;
        private static readonly float MaxCircleImageScale = 1.06f;
        private static readonly Vector3 ThunderRotateVector3 = new Vector3(0.0f, 0.0f, 8.0f);

        private void Awake()
        {
            // Alpha値を上げるSequenceを作成
            var appearSequence = DOTween.Sequence();
            appearSequence.AppendCallback(() => backThunderImage.gameObject.SetActive(false)); // 最初はイナズマ非表示
            appearSequence.Append(CreateToAlphaTween(whiteBackImage, 1.0f));
            appearSequence.Join(CreateToAlphaTween(frontTextImage, 1.0f));

            backCircleImageList.ForEach(backCircleImage =>
            {
                appearSequence.Join(CreateToAlphaTween(backCircleImage, 1.0f));
            });

            // イナズマを遅れてひょいと出す
            var thunderAppearSequence = DOTween.Sequence();
            thunderAppearSequence.AppendInterval(0.6f);
            thunderAppearSequence.AppendCallback(() =>
            {
                backThunderImage.transform.localEulerAngles = ThunderRotateVector3;
                backThunderImage.gameObject.SetActive(true);
            });
            thunderAppearSequence.Join(CreateToAlphaTween(backThunderImage, 0.5f));
            thunderAppearSequence.Join(backThunderImage.transform.DOLocalRotate(Vector3.zero, 0.5f));
            appearSequence.Join(thunderAppearSequence);

            // 大きさを徐々に変えるループアニメーションを作成
            _loopTweenList = new List<Tween>();
            foreach (var backCircleImage in backCircleImageList)
            {
                // 間隔と開始時間を適当にばらまく
                var randomDelayTime = Random.Range(0.0f, 0.5f);
                var randomDurationTime = Random.Range(1.0f, 1.5f);
                backCircleImage.transform.localScale = Vector3.one * MinCircleImageScale;
                _loopTweenList.Add(
                    backCircleImage.transform.DOScale(MaxCircleImageScale, randomDurationTime)
                        .SetDelay(randomDelayTime).SetLoops(-1, LoopType.Yoyo));
            }

            // イナズマのループアニメーション
            var thunderLoopSequence = DOTween.Sequence();
            thunderLoopSequence.AppendInterval(0.8f);
            thunderLoopSequence.Append(backThunderImage.transform.DOLocalRotate(ThunderRotateVector3, 0.2f));
            thunderLoopSequence.Append(backThunderImage.transform.DOLocalRotate(Vector3.zero, 0.2f));
            thunderLoopSequence.SetLoops(-1, LoopType.Restart);
            thunderLoopSequence.Pause();
            DOTween.Sequence().AppendCallback(() => thunderLoopSequence.Restart()).SetDelay(1.0f);
            _loopTweenList.Add(thunderLoopSequence);
        }

        private Tween CreateToAlphaTween(Image targetImage, float duration)
        {
            // alphaを0に設定
            var color = targetImage.color;
            color.a = 0.0f;
            targetImage.color = color;
            // alphaを1まで上げるtweenを返す
            return DOTween.ToAlpha(
                () => targetImage.color,
                c => targetImage.color = c,
                1.0f,
                duration);

        }

        private void OnDestroy()
        {
            foreach (var tween in _loopTweenList)
            {
                tween.Kill();
            }
        }
    }
}
