using System;
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
        [SerializeField] private List<TitleSelectPlayerToggle> selectPlayerToggles;
        public void ResetSelectPlayerToggles()
        {
            // 全ての選択状態をfalseにする
            foreach (var selectPlayerToggle in selectPlayerToggles)
            {
                selectPlayerToggle.SetIsOn(false);
            }
            // 先頭のToggle(None)を選択状態にする
            selectPlayerToggles[0].SetIsOn(true);
        }
        public void SetListenerSelectPlayerToggles(UnityAction<bool, PlayerType> action)
        {
            foreach (var selectPlayerToggle in selectPlayerToggles)
            {
                selectPlayerToggle.SetListener(action);
            }
        }

        /// <summary>
        /// 決定ボタン
        /// </summary>
        [SerializeField] private Button decideButton;
        public void SetInteractableDecideButton(bool isActive)
        {
            decideButton.interactable = isActive;
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
