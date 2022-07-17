using System;
using Cysharp.Threading.Tasks;
using Reversi.Managers;
using Reversi.Stones.Stone;
using UnityEngine;
using Object = UnityEngine.Object;

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
        /// ストーン選択処理を待機するか？
        /// </summary>
        private bool _isWaitSelect;
        bool IPlayer.IsWaitSelect
        {
            get => _isWaitSelect;
            set => _isWaitSelect = value;
        }

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
        /// プレイヤーのゲームオブジェクト
        /// </summary>
        protected GameObject PlayerGameObject;

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
        public void OnStartTurn(StoneState[,] stoneStates)
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
        public void OnUpdateTurn()
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
        /// ゲーム終了処理
        /// </summary>
        public void OnEndGame(PlayerResultState resultState)
        {
            EndGame(resultState);
        }
        protected virtual void EndGame(PlayerResultState resultState) { }

        /// <summary>
        /// ストーン選択待機処理
        /// </summary>
        /// <param name="waitMs"></param>
        protected async UniTask WaitSelectTime(int waitMs)
        {
            if (_isWaitSelect)
            {
                await UniTask.Delay(waitMs);
            }
        }

        /// <summary>
        /// 入力プレイヤーかどうか？
        /// </summary>
        public virtual bool IsInputPlayer()
        {
            // 基本的にfalse
            return false;
        }

        /// <summary>
        /// ゲームオブジェクトを生成する
        /// </summary>
        /// <param name="obj"></param>
        public void OnInstantiate(GameObject obj)
        {
            PlayerGameObject = Object.Instantiate(obj);
        }

        /// <summary>
        /// オブジェクトを破棄する
        /// プレイヤー切り替えの際に呼び出す
        /// </summary>
        public void OnDestroy()
        {
            if (PlayerGameObject != null)
            {
                Object.Destroy(PlayerGameObject);
            }
        }
    }
}
