using System;
using Reversi.Players;
using Reversi.Settings;
using Reversi.Stones.Stone;
using VContainer;

namespace Reversi.Managers
{
    /// <summary>
    /// プレイヤー管理クラス
    /// </summary>
    public class PlayerManager
    {
        private readonly StoneManager _stoneManager;
        private readonly PlayerFactory _playerFactory;
        private readonly GameSettings _gameSettings;

        /// <summary>
        /// プレイヤー
        /// </summary>
        private IPlayer _player1;
        private IPlayer _player2;

        /// <summary>
        /// アクティブプレイヤー
        /// </summary>
        private IPlayer _activePlayer;

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        private Action _changeNextTurnAction;

        [Inject]
        public PlayerManager(StoneManager stoneManager, PlayerFactory playerFactory, GameSettings gameSettings)
        {
            _stoneManager = stoneManager;
            _playerFactory = playerFactory;
            _gameSettings = gameSettings;
        }

        /// <summary>
        /// ゲーム初期化
        /// </summary>
        /// <param name="player1Type">プレイヤー1</param>
        /// <param name="player2Type">プレイヤー2</param>
        /// <param name="changeNextTurnAction">ターン切り替え処理</param>
        public void InitializeGame(PlayerType player1Type, PlayerType player2Type, Action changeNextTurnAction)
        {
            _changeNextTurnAction = changeNextTurnAction;

            // プレイヤー作成
            // 学習中の場合は最初の一回のみ作成
            if (!_gameSettings.debugOption.isLearnAgent || _activePlayer == null)
            {
                _player1 = _playerFactory.CreatePlayer(player1Type, StoneState.White, PutStone);
                _player2 = _playerFactory.CreatePlayer(player2Type, StoneState.Black, PutStone);

                // デバッグ情報の設定
                _player1.IsWaitSelect = _gameSettings.debugOption.isWaitSelectStone;
                _player2.IsWaitSelect = _gameSettings.debugOption.isWaitSelectStone;
            }
            _activePlayer = _player1;
        }

        public void EndGame()
        {
            // プレイヤーを破棄する
            _player1.OnEndGame(GetPlayer1ResultState());
            _player2.OnEndGame(GetPlayer2ResultState());
            // 学習中の場合には破棄しない
            if (!_gameSettings.debugOption.isLearnAgent)
            {
                _player1.OnDestroy();
                _player2.OnDestroy();
                _activePlayer = null;
            }
        }

        /// <summary>
        /// ターン開始処理
        /// </summary>
        public void StartTurn()
        {
            // 入力プレイヤーの場合、置けるストーンをフォーカスする
            if (_activePlayer.IsInputPlayer()) _stoneManager.SetFocusAllCanPutStones(_activePlayer.MyStoneState);
            // ターン開始
            _activePlayer.OnStartTurn(_stoneManager.GetStoneStatesClone());
        }

        /// <summary>
        /// ターン更新処理
        /// </summary>
        public void UpdateTurn()
        {
            // ターン更新
            _activePlayer.OnUpdateTurn();
        }

        /// <summary>
        /// プレイヤー切り替え
        /// </summary>
        /// <param name="turnCount"></param>
        public void ChangePlayer(int turnCount)
        {
            var isFirstTurn = turnCount % 2 == 0;
            _activePlayer = isFirstTurn ? _player1 : _player2;
        }

        /// <summary>
        /// アクティブプレイヤーがストーンを置けるかどうか？
        /// </summary>
        /// <returns></returns>
        public bool IsCanPutStoneActivePlayer()
        {
            return _stoneManager.GetAllCanPutStonesIndex(_activePlayer.MyStoneState).Count > 0;
        }

        /// <summary>
        /// プレイヤー1の結果状態を返却する
        /// </summary>
        public PlayerResultState GetPlayer1ResultState()
        {
            return _stoneManager.GetPlayerResultState(_player1.MyStoneState);
        }

        /// <summary>
        /// プレイヤー2の結果状態を返却する
        /// </summary>
        public PlayerResultState GetPlayer2ResultState()
        {
            return _stoneManager.GetPlayerResultState(_player2.MyStoneState);
        }

        /// <summary>
        /// ストーンを置く
        /// </summary>
        /// <param name="stoneState"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        private void PutStone(StoneState stoneState, int x, int z)
        {
            // ストーン置く
            if (_stoneManager.PutStone(stoneState, x, z))
            {
                // フォーカス解除
                _stoneManager.ReleaseFocusStones();

                // ターン切り替え
                _changeNextTurnAction();
            }
        }
    }
}
