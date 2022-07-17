using System;
using Reversi.Players;
using UnityEngine;

namespace Reversi.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Reversi/GameSettings")]
    public class GameSettings : ScriptableObject
    {
        /// <summary>
        /// デバッグオプション
        /// </summary>
        public DebugOption debugOption;
        /// <summary>
        /// プレイヤー
        /// </summary>
        public PlayerType initPlayer1;
        public PlayerType initPlayer2;
    }

    [Serializable]
    public class DebugOption
    {
        /// <summary>
        /// ゲームをループさせるか？
        /// </summary>
        public bool isGameLoop;

        /// <summary>
        /// アニメーションを表示させるか？
        /// </summary>
        public bool isDisplayAnimation;

        /// <summary>
        /// ストーン選択処理を待機させるか？
        /// </summary>
        public bool isWaitSelectStone;

        /// <summary>
        /// デバッグログをテキストに出力するか？
        /// </summary>
        public bool isWriteDebugLog;

        /// <summary>
        /// エージェントを学習させるか？
        /// </summary>
        public bool isLearnAgent;
    }
}
