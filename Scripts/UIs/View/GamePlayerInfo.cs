using Reversi.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    public class GamePlayerInfo : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI stoneCountText;
        [SerializeField] private Image borderImage;

        private readonly Color ColorActive = Color.yellow;
        private readonly Color ColorNormal = Color.white;

        public void SetPlayerName(string playerName)
        {
            playerNameText.SetTextForDynamic(playerName);
        }

        public void SetStoneCount(int count)
        {
            stoneCountText.SetTextForDynamic(count.ToString());
        }

        /// <summary>
        /// アクティブかどうか分かるよう、色を変える
        /// </summary>
        /// <param name="isActive"></param>
        public void SetActiveColor(bool isActive)
        {
            var changeColor = isActive ? ColorActive : ColorNormal;
            borderImage.color = changeColor;
            playerNameText.color = changeColor;
        }
    }
}
