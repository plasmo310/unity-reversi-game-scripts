using System;
using Reversi.Stones.Stone;

namespace Reversi.Players
{
    /// <summary>
    /// プレイヤー共通クラス
    /// </summary>
    public abstract class Player : IPlayer
    {
        /// <summary>
        /// 自分のストーン状態(黒or白)
        /// </summary>
        protected readonly StoneState MyStoneState;
        StoneState IPlayer.MyStoneState => MyStoneState;

        /// <summary>
        /// ストーン配列(AIチェック用)
        /// </summary>
        protected StoneState[,] StoneStates;

        /// <summary>
        /// 選択したストーン
        /// 継承先のクラスでこの変数に設定する
        /// </summary>
        protected StoneIndex SelectStoneIndex;

        /// <summary>
        /// ストーン選択処理
        /// </summary>
        private readonly Action<StoneState, int, int> _putStoneAction;

        protected Player(StoneState myStoneState, Action<StoneState, int, int> putStoneAction)
        {
            _putStoneAction = putStoneAction;
            MyStoneState = myStoneState;
        }

        /// <summary>
        /// ターン開始
        /// </summary>
        /// <param name="stoneStates"></param>
        public void StartTurn(StoneState[,] stoneStates)
        {
            // ストーン配列を設定して初期化
            StoneStates = stoneStates;
            SelectStoneIndex = null;

            // 思考開始
            StartThink();
        }

        /// <summary>
        /// ターン更新
        /// </summary>
        public void UpdateTurn()
        {
            // 思考更新
            UpdateThink();

            // ストーンが選択されたら置く
            if (SelectStoneIndex == null) return;
            _putStoneAction(MyStoneState, SelectStoneIndex.X, SelectStoneIndex.Z);
            SelectStoneIndex = null; // 初期化
        }

        /// <summary>
        /// 思考処理
        /// </summary>
        protected virtual void StartThink() { }
        protected virtual void UpdateThink() { }

        /// <summary>
        /// 入力プレイヤーかどうか？
        /// </summary>
        public virtual bool IsInputPlayer()
        {
            // 基本的にfalse
            return false;
        }
    }
}
