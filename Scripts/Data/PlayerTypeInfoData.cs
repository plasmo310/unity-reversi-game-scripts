using System;
using System.Collections.Generic;
using Reversi.Players;
using UnityEngine;

namespace Reversi.Data
{
    [CreateAssetMenu(fileName = "PlayerTypeInfoData", menuName = "Reversi/PlayerTypeInfoData")]
    public class PlayerTypeInfoData : ScriptableObject
    {
        /// <summary>
        /// 各プレイヤー情報
        /// </summary>
        [SerializeField] private List<PlayerTypeInfo> playerTypeInfos;
        [Serializable] public class PlayerTypeInfo
        {
            /// <summary>
            /// プレイヤー種類
            /// </summary>
            public PlayerType playerType;
            /// <summary>
            /// 名前
            /// </summary>
            public string name;
            /// <summary>
            /// 出席番号
            /// </summary>
            public string studentNo;
            /// <summary>
            /// 詳細情報
            /// </summary>
            public string detail;
            /// <summary>
            /// オプションラベル
            /// </summary>
            public string optionalLabel;
            /// <summary>
            /// アイコン
            /// </summary>
            public Sprite iconSprite;
            /// <summary>
            /// イメージカラー
            /// </summary>
            public Color imageColor;
        }

        /// <summary>
        /// 指定のプレイヤー情報を返却する
        /// </summary>
        /// <param name="playerType"></param>
        public PlayerTypeInfo GetPlayerTypeInfo(PlayerType playerType)
        {
            var playerTypeInfo = playerTypeInfos.Find(x => x.playerType == playerType);
            if (playerTypeInfo == null)
            {
                playerTypeInfo = new PlayerTypeInfo
                {
                    playerType = PlayerType.None,
                    name = "",
                    studentNo = "",
                    detail = "",
                    optionalLabel = "",
                    iconSprite = null,
                    imageColor = Color.white
                };
            }
            return playerTypeInfo;
        }
    }
}
