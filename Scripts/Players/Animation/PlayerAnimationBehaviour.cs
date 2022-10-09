using Reversi.Const;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reversi.Players.Animation
{
    /// <summary>
    /// プレイヤーアニメーションBehaviour
    /// </summary>
    public class PlayerAnimationBehaviour : MonoBehaviour
    {
        /// <summary>
        /// アニメーション関連
        /// </summary>
        private Animator _animator;
        private static readonly string AnimParamEmotion = "EmotionInt";
        private static readonly string AnimParamResult = "ResultInt";
        private static readonly string AnimParamPutTrigger = "PutTrigger";
        private static readonly string AnimParamWaitBool = "WaitBool";

        private void Awake()
        {
            // Animatorを取得
            _animator = gameObject.GetComponent<Animator>();

            StartAnimation();
        }

        private void OnEnable()
        {
            StartAnimation();
        }

        private void StartAnimation()
        {
            // タイトル画面の場合、Waitから開始する
            if (SceneManager.GetActiveScene().name == GameConst.SceneNameTitle)
            {
                _animator.SetBool(AnimParamWaitBool, true);
            }
        }

        /// <summary>
        /// Putアニメーション開始
        /// </summary>
        public void StartPutAnimation()
        {
            _animator.SetTrigger(AnimParamPutTrigger);
        }

        /// <summary>
        /// 感情値を設定
        /// </summary>
        /// <param name="emotion"></param>
        public void SetEmotionInt(int emotion)
        {
            _animator.SetInteger(AnimParamEmotion, emotion);
        }

        /// <summary>
        /// 結果値を設定
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(int result)
        {
            _animator.SetInteger(AnimParamResult, result);
        }
    }
}
