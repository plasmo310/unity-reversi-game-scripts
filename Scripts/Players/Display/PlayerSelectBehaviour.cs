using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi.Players.Display
{
    public class PlayerSelectBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 各プレイヤーごとの設定
        /// </summary>
        [SerializeField] private List<SelectPlayerInfo> selectPlayerInfos;
        [Serializable] private class SelectPlayerInfo
        {
            public PlayerType playerType;
            public GameObject playerObject;
        }

        private PlayerType _selectPlayerType;

        private void Awake()
        {
            _selectPlayerType = PlayerType.None;
            // 最初は全て表示する
            HideAllSelectPlayer();
        }

        /// <summary>
        /// 指定プレイヤーを表示する
        /// </summary>
        /// <param name="playerType">プレイヤー種類</param>
        public void ShowSelectPlayer(PlayerType playerType)
        {
            // 選択済なら何も行わない
            if (_selectPlayerType == playerType) return;
            _selectPlayerType = playerType;

            // Noneは全体表示のため全て表示する
            if (playerType == PlayerType.None)
            {
                ShowAllSelectPlayer();
                return;
            }
            // 指定されたプレイヤーのみ表示する
            selectPlayerInfos.ForEach(x => x.playerObject.SetActive(false));
            selectPlayerInfos.Find(x => x.playerType == playerType).playerObject.SetActive(true);
        }

        /// <summary>
        /// 全てのプレイヤーを表示する
        /// </summary>
        public void ShowAllSelectPlayer()
        {
            foreach (var selectPlayerInfo in selectPlayerInfos)
            {
                selectPlayerInfo.playerObject.SetActive(true);
            }
        }

        /// <summary>
        /// 全てのプレイヤーを非表示にする
        /// </summary>
        public void HideAllSelectPlayer()
        {
            foreach (var selectPlayerInfo in selectPlayerInfos)
            {
                selectPlayerInfo.playerObject.SetActive(false);
            }
        }
    }
}
