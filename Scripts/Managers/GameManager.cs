using System;
using Cysharp.Threading.Tasks;
using Reversi.Common;
using Reversi.Players;
using Reversi.Services;
using Reversi.Settings;
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
        private readonly GameSettings _gameSettings;
        private readonly ILogService _logService;

        /// <summary>
        /// 選択プレイヤー
        /// </summary>
        private PlayerType _selectPlayer1Type;
        private PlayerType _selectPlayer2Type;

        /// <summary>
        /// ゲーム状態
        /// </summary>
        private readonly ReactiveProperty<GameState> _state = new(GameState.Select);
        public IReadOnlyReactiveProperty<GameState> State => _state;
        private readonly StateMachine<GameManager> _stateMachine;

        [Inject]
        public GameManager(BoardManager boardManager, StoneManager stoneManager, PlayerManager playerManager, GameSettings gameSettings, ILogService logService)
        {
            _boardManager = boardManager;
            _stoneManager = stoneManager;
            _playerManager = playerManager;
            _gameSettings = gameSettings;
            _logService = logService;

            // ステートマシン設定
            _stateMachine = new StateMachine<GameManager>(this);
            _stateMachine.SetChangeStateEvent((stateId) =>
            {
                // ステート変更時にReactivePropertyにも反映
                _state.Value = (GameState) Enum.ToObject(typeof(GameState), stateId);
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
                Owner._selectPlayer1Type = Owner._gameSettings.initPlayer1;
                Owner._selectPlayer2Type = Owner._gameSettings.initPlayer2;

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
                Owner._playerManager.InitializeGame(Owner._selectPlayer1Type, Owner._selectPlayer2Type, ChangeNextTurn);
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
                        EndGame();
                        return;
                    }

                    // TODO キャラクターが反応するようにする
                    // アニメーション再生分、少し待機
                    if (Owner._gameSettings.debugOption.isDisplayAnimation)
                    {
                        await UniTask.Delay(350);
                    }

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

            /// <summary>
            /// ゲーム終了
            /// </summary>
            private void EndGame()
            {
                Owner._playerManager.EndGame();
                StateMachine.ChangeState((int)GameState.Result);
            }
        }

        // ----- 結果表示 -----
        private class StateResult : StateMachine<GameManager>.StateBase
        {
            public override void OnStart()
            {
                // TODO UIに結果を表示する
                Owner._logService.PrintLog(
                    "player1=>" + Owner._playerManager.GetPlayer1ResultState().ToString() +
                    " black: " + Owner._stoneManager.BlackStoneCount + " white: " + Owner._stoneManager.WhiteStoneCount);
            }
            public override void OnUpdate()
            {
                // Return押下でもう一度プレイ
                if (Input.GetKeyDown(KeyCode.Return) || Owner._gameSettings.debugOption.isGameLoop)
                {
                    StateMachine.ChangeState((int) GameState.Play);
                }
            }
            public override void OnEnd() { }
        }
    }
}
