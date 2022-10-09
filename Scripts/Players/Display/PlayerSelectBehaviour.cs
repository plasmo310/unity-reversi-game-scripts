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
        }

        /// <summary>
        /// 表示対象のプレイヤーを設定する
        /// </summary>
        private List<PlayerType> _showTargetPlayerTypes;
        public void SetShowTargetPlayerType(PlayerType playerType)
        {
            if (_showTargetPlayerTypes == null)
            {
                _showTargetPlayerTypes = new List<PlayerType>();
            }
            _showTargetPlayerTypes.Add(playerType);
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
            selectPlayerInfos.ForEach(x =>
            {
                var isDisplay = x.playerType == playerType;
                // AIRobotの場合、いずれかの種別ならtrue
                if (PlayerTypeUtil.IsAiRobotType(playerType))
                {
                    isDisplay = PlayerTypeUtil.IsAiRobotType(x.playerType);
                }
                ChangeDisplayPlayer(x, isDisplay);
            });
        }

        /// <summary>
        /// 全てのプレイヤーを表示する
        /// </summary>
        public void ShowAllSelectPlayer()
        {
            foreach (var selectPlayerInfo in selectPlayerInfos)
            {
                // 表示対象外のプレイヤーは省く
                if (!_showTargetPlayerTypes.Contains(selectPlayerInfo.playerType)) return;
                ChangeDisplayPlayer(selectPlayerInfo, true);
            }
        }

        /// <summary>
        /// 全てのプレイヤーを非表示にする
        /// </summary>
        public void HideAllSelectPlayer()
        {
            foreach (var selectPlayerInfo in selectPlayerInfos)
            {
                ChangeDisplayPlayer(selectPlayerInfo, false);
            }
        }

        /// <summary>
        /// プレイヤーの表示を切り替える
        /// </summary>
        /// <param name="playerInfo"></param>
        /// <param name="isDisplay"></param>
        private void ChangeDisplayPlayer(SelectPlayerInfo playerInfo, bool isDisplay)
        {
            if (playerInfo.playerObject.activeSelf != isDisplay)
            {
                playerInfo.playerObject.SetActive(isDisplay);
            }
        }
    }
}
