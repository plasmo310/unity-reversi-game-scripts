using DG.Tweening;
using Reversi.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    /// <summary>
    /// 共通ダイアログView
    /// </summary>
    public class DialogView : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        [SerializeField] private RectTransform dialogRectTransform;
        [SerializeField] private Image maskImage;

        /// <summary>
        /// ダイアログ表示
        /// </summary>
        public void OnShow()
        {
            // 初期設定
            dialogRectTransform.localScale = Vector3.zero;
            dialogRectTransform.localEulerAngles = Vector3.zero;

            // 徐々に大きくしながら表示
            maskImage.gameObject.SetActive(true);
            gameObject.SetActive(true);
            dialogRectTransform.DOScale(1.0f, 0.3f).SetEase(Ease.OutExpo);
        }

        /// <summary>
        /// ダイアログ非表示
        /// </summary>
        public void OnHide()
        {
            // 徐々に小さくしながら非表示
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() => maskImage.gameObject.SetActive(false)); // マスクは先に消す
            sequence.Append(dialogRectTransform.DOScale(0.0f, 0.3f).SetEase(Ease.OutQuart));
            sequence.AppendCallback(() => gameObject.SetActive(false)); // 全体を非表示
        }

        /// <summary>
        /// メッセージ
        /// </summary>
        [SerializeField] private TextMeshProUGUI dialogMessageText;
        public void SetDialogMessageText(string text)
        {
            dialogMessageText.SetTextForDynamic(text);
        }

        /// <summary>
        /// OKボタン
        /// </summary>
        [SerializeField] private Button okButton;
        [SerializeField] private TextMeshProUGUI okButtonText;
        public void SetListenerOkButton(UnityAction action)
        {
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(action);
        }
        public void SetTextOkButtonText(string text)
        {
            okButtonText.SetTextForDynamic(text);
        }
    }
}
