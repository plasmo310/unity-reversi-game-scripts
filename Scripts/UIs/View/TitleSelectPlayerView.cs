using System.Collections.Generic;
using Reversi.Extensions;
using Reversi.Players;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    /// <summary>
    /// プレイヤー選択View
    /// </summary>
    public class TitleSelectPlayerView : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// プレイヤー選択Toggles
        /// </summary>
        private readonly List<TitleSelectPlayerToggle> _selectPlayerToggles = new List<TitleSelectPlayerToggle>();

        [SerializeField] private ToggleGroup selectPlayerToggleGroup;
        [SerializeField] private GameObject selectPlayerTogglePrefab;
        public void CreatePlayerToggle(PlayerType playerType, Sprite playerSprite, Color playerColor, string optionalText, bool isRelease)
        {
            // Toggleを生成して追加
            var playerToggle = Instantiate(selectPlayerTogglePrefab, selectPlayerToggleGroup.transform)
                .GetComponent<TitleSelectPlayerToggle>();
            playerToggle.InitializeToggle(selectPlayerToggleGroup, playerType, playerSprite, playerColor, optionalText, isRelease);
            _selectPlayerToggles.Add(playerToggle);
        }
        public void ResetSelectPlayerToggles()
        {
            // 全ての選択状態をfalseにする
            foreach (var selectPlayerToggle in _selectPlayerToggles)
            {
                selectPlayerToggle.SetIsOn(false);
            }
            // 先頭のToggle(None)を選択状態にする
            _selectPlayerToggles[0].SetIsOn(true);
        }
        public void SetListenerSelectPlayerToggles(UnityAction<bool, PlayerType> action)
        {
            foreach (var selectPlayerToggle in _selectPlayerToggles)
            {
                selectPlayerToggle.SetListener(action);
            }
        }

        /// <summary>
        /// 選択メッセージ
        /// </summary>
        [SerializeField] private TextMeshProUGUI selectMessageText;
        public void SetActiveSelectMessageText(bool isActive)
        {
            selectMessageText.gameObject.SetActive(isActive);
        }
        public void SetTextSelectMessageText(string text)
        {
            selectMessageText.SetTextForDynamic(text);
        }

        /// <summary>
        /// 決定ボタン
        /// </summary>
        [SerializeField] private Button decideButton;
        public void SetActiveDecideButton(bool isActive)
        {
            decideButton.gameObject.SetActive(isActive);
        }
        public void SetListenerDecideButton(UnityAction action)
        {
            decideButton.onClick.RemoveAllListeners();
            decideButton.onClick.AddListener(action);
        }

        /// <summary>
        /// プレイヤー詳細
        /// </summary>
        [SerializeField] private GameObject playerDetailArea;
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerStudentNoText;
        [SerializeField] private TextMeshProUGUI playerDetailText;
        public void SetActivePlayerDetailArea(bool isActive)
        {
            playerDetailArea.SetActive(isActive);
        }
        public void SetPlayerDetailText(string name, string studentNo, string detail)
        {
            playerNameText.SetTextForDynamic(name);
            playerStudentNoText.SetTextForDynamic(studentNo);
            playerDetailText.SetTextForDynamic(detail);
        }
    }
}
