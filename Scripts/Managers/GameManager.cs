using System;
using Cysharp.Threading.Tasks;
using Reversi.Common;
using Reversi.Players;
using UniRx;
using UnityEngine;
using VContainer;

namespace Reversi.Managers
{
    /// <summary>
    /// ゲーム管理クラス
    /// </summary>
    public class GameManager : IDisposable
    {
        private readonly BoardManager _boardManager;
        private readonly StoneManager _stoneManager;
        private readonly PlayerManager _playerManager;

        /// <summary>
        /// 選択プレイヤー
        /// </summary>
        private PlayerKind _selectPlayer1;
        private PlayerKind _selectPlayer2;

        /// <summary>
        /// ゲーム状態
        /// </summary>
        private readonly ReactiveProperty<GameState> _state = new(GameState.Select);
        public IReadOnlyReactiveProperty<GameState> State => _state;
        private readonly StateMachine<GameManager> _stateMachine;

        [Inject]
        public GameManager(BoardManager boardManager, StoneManager stoneManager, PlayerManager playerManager)
        {
            _boardManager = boardManager;
            _stoneManager = stoneManager;
            _playerManager = playerManager;

            // ステートマシン設定
            _stateMachine = new StateMachine<GameManager>(this);
            _stateMachine.SetChangeStateEvent((stateId) =>
            {
                // ステート変更時にReactivePropertyにも反映
                _state.Value = (GameState) Enum.ToObject(typeof(GameState), stateId);
                Debug.Log("Change: " + _state);
            });
            _stateMachine.Add<StateSelect>((int) GameState.Select);
            _stateMachine.Add<StatePlay>((int) GameState.Play);
            _stateMachine.Add<StateResult>((int) GameState.Result);
        }

        public void OnStart()
        {
            _stateMachine.OnStart((int) GameState.Select);
        }

        public void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }

        public void Dispose()
        {
            // 購読解除
            _state.Dispose();
        }

        // ----- 選択中 -----
        private class StateSelect : StateMachine<GameManager>.StateBase
        {
            public override void OnStart()
            {
                // ボードの生成
                Owner._boardManager.GenerateBoard();

                // TODO 自由に切り替えられるようにする
                // プレイヤーの選択
                Owner._selectPlayer1 = PlayerKind.InputPlayer;
                Owner._selectPlayer2 = PlayerKind.MiniMaxAIPlayer;

                // ゲーム開始
                StateMachine.ChangeState((int) GameState.Play);
            }
            public override void OnUpdate() { }
            public override void OnEnd() { }
        }

        // ----- プレイ中 -----
        private class StatePlay : StateMachine<GameManager>.StateBase
        {
            private int _tuneCount = 0;
            public override void OnStart()
            {
                _tuneCount = 0;

                // ストーン初期化
                Owner._stoneManager.InitializeStones(BoardManager.CellSideCount, Owner._boardManager.GetCellPosition);

                // ゲーム初期化してターン開始
                Owner._playerManager.InitializeGame(Owner._selectPlayer1, Owner._selectPlayer2, ChangeNextTurn);
                Owner._playerManager.StartTurn();
            }

            public override void OnUpdate()
            {
                // ターン更新
                Owner._playerManager.UpdateTurn();
            }
            public override void OnEnd() { }

            /// <summary>
            /// 次のターンに変更する
            /// </summary>
            private async void ChangeNextTurn()
            {
                while (true)
                {
                    _tuneCount++;

                    //  全てのストーンが置けない場合、ゲーム終了
                    if (!Owner._stoneManager.IsCanPutStone())
                    {
                        StateMachine.ChangeState((int)GameState.Result);
                        return;
                    }

                    // TODO キャラクターが反応するようにする
                    // 少し待機
                    await UniTask.Delay(350);

                    // プレイヤーの切り替え
                    Owner._playerManager.ChangePlayer(_tuneCount);

                    // プレイヤーがストーンを置けない場合、次のターンに変更
                    if (!Owner._playerManager.IsCanPutStoneActivePlayer())
                    {
                        continue;
                    }

                    // 次のターン開始
                    Owner._playerManager.StartTurn();
                    break;
                }
            }
        }

        // ----- 結果表示 -----
        private class StateResult : StateMachine<GameManager>.StateBase
        {
            public override void OnStart()
            {
                // TODO ちゃんと結果を表示する
                Debug.Log("Finish Game!!");
            }
            public override void OnUpdate()
            {
                // Return押下でもう一度プレイ
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    StateMachine.ChangeState((int) GameState.Play);
                }
            }
            public override void OnEnd() { }
        }
    }
}
