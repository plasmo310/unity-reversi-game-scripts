using Reversi.Players;
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
        [SerializeField] private PlayerType selectPlayerType;
        [SerializeField] private Toggle toggle;

        /// <summary>
        /// Toggle切替処理の設定
        /// </summary>
        public void SetListener(UnityAction<bool, PlayerType> action)
        {
            // 設定されたPlayerTypeも渡す
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(isOn =>
            {
                action(isOn, selectPlayerType);
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
