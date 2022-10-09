using System;
using Reversi.Managers;
using Reversi.Players;
using UnityEngine;

namespace Reversi.Settings
{
    [CreateAssetMenu(fileName = "GameSettings", menuName = "Reversi/GameSettings")]
    public class GameSettings : ScriptableObject, ISerializationCallbackReceiver
    {
        /// <summary>
        /// デバッグオプション
        /// </summary>
        [SerializeField] private GameDebugOption debugOption;
        public GameDebugOption DebugOption => debugOption;

        /// <summary>
        /// モバイルプラットフォームか？
        /// </summary>
        [NonSerialized] public bool IsMobilePlatform;

        /// <summary>
        /// 選択したゲームモード
        /// </summary>
        [SerializeField] private GameModeType initSelectGameMode;
        [NonSerialized] public GameModeType SelectGameModeType;

        /// <summary>
        /// 選択したプレイヤー
        /// </summary>
        [SerializeField] private PlayerType initSelectPlayer1;
        [SerializeField] private PlayerType initSelectPlayer2;
        [NonSerialized] public PlayerType SelectPlayer1;
        [NonSerialized] public PlayerType SelectPlayer2;

        /// <summary>
        /// プレイヤー位置
        /// ※ランタイムでのみ設定
        /// </summary>
        [NonSerialized] public Transform Player1Transform;
        [NonSerialized] public Transform Player2Transform;
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize()
        {
            // ランタイムでの書き込み用に値をコピーする
            SelectGameModeType = initSelectGameMode;
            SelectPlayer1 = initSelectPlayer1;
            SelectPlayer2 = initSelectPlayer2;
        }
    }

    [Serializable]
    public class GameDebugOption
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
