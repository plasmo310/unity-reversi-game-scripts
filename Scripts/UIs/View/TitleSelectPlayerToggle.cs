using System;
using Reversi.Extensions;
using Reversi.Players;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    /// <summary>
    /// プレイヤー選択Toggle
    /// </summary>
    public class TitleSelectPlayerToggle : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Toggle toggle;
        [SerializeField] private Image playerImage;
        [SerializeField] private Image bgImage;
        [SerializeField] private GameObject checkMark;
        [SerializeField] private GameObject secretMark;
        [SerializeField] private GameObject optionalLabel;
        [SerializeField] private TextMeshProUGUI optionalText;
        private PlayerType _playerType;

        public void InitializeToggle(ToggleGroup toggleGroup, PlayerType playerType, Sprite playerSprite,
            Color playerColor, string playerOptionalText, bool isRelease)
        {
            toggle.group = toggleGroup;
            _playerType = playerType;

            // None指定の場合のみ、サイズを0にして返却
            if (playerType == PlayerType.None)
            {
                rectTransform.sizeDelta = Vector2.zero;
                playerImage.sprite = null;
                optionalLabel.SetActive(false);
                secretMark.SetActive(false);
                Destroy(checkMark); // チェックマークも外す
                return;
            }

            // トグルの設定
            playerImage.sprite = playerSprite;
            bgImage.color = playerColor;

            // オプション文字列が指定されていた場合
            if (!String.IsNullOrEmpty(playerOptionalText))
            {
                optionalLabel.SetActive(true);
                optionalText.SetTextForDynamic(playerOptionalText);
            }
            else
            {
                optionalLabel.SetActive(false);
            }

            // 未解放の場合
            if (!isRelease)
            {
                toggle.interactable = false;
                playerImage.color = Color.black;
                bgImage.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
                optionalLabel.SetActive(false);
                secretMark.SetActive(true);
            }
            else
            {
                toggle.interactable = true;
                secretMark.SetActive(false);
            }
        }

        /// <summary>
        /// Toggle切替処理の設定
        /// </summary>
        public void SetListener(UnityAction<bool, PlayerType> action)
        {
            // 設定されたPlayerTypeも渡す
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(isOn =>
            {
                action(isOn, _playerType);
            });
        }

        /// <summary>
        /// チェックの設定
        /// </summary>
        public void SetIsOn(bool isOn)
        {
            toggle.isOn = isOn;
        }
    }
}
