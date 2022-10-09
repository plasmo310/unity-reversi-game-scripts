using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    public class TransitionView : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// マスク切り抜き画像
        /// </summary>
        [SerializeField] private Image unMaskImage;

        private readonly Vector3 _maxMaskScale = Vector3.one * 10.0f;
        private readonly Vector3 _minMaskScale = Vector3.zero;

        /// <summary>
        /// 遷移中か？
        /// </summary>
        private bool _isDoTransition = false;

        /// <summary>
        /// 遷移開始
        /// </summary>
        /// <param name="callback"></param>
        public void StartTransition(UnityAction callback)
        {
            if (_isDoTransition) return;
            _isDoTransition = true;

            gameObject.SetActive(true);
            unMaskImage.transform.localScale = _maxMaskScale;

            // フェードさせてcallbackを実行
            var sequence = DOTween.Sequence();
            sequence.Append(unMaskImage.transform.DOScale(_minMaskScale, 0.5f).SetEase(Ease.OutExpo));
            sequence.AppendInterval(0.2f);
            sequence.AppendCallback(() =>
            {
                callback();
            });
        }

        /// <summary>
        /// 遷移終了
        /// </summary>
        public void EndTransition()
        {
            unMaskImage.transform.localScale = _minMaskScale;

            // フェードさせて非表示にする
            var sequence = DOTween.Sequence();
            sequence.Append(unMaskImage.transform.DOScale(_maxMaskScale, 1.0f).SetEase(Ease.InExpo));
            sequence.AppendCallback(() =>
            {
                gameObject.SetActive(false);
                _isDoTransition = false;
            });
        }
    }
}
