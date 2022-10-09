using System;
using Reversi.Data;
using Reversi.Players;
using Reversi.Settings;
using Reversi.Stones.Stone;
using UniRx;
using VContainer;

namespace Reversi.Managers
{
    /// <summary>
    /// プレイヤー管理クラス
    /// </summary>
    public class PlayerManager : IDisposable
    {
        private readonly StoneManager _stoneManager;
        private readonly PlayerFactory _playerFactory;
        private readonly GameSettings _gameSettings;
        private readonly PlayerTypeInfoData _playerTypeInfoData;

        /// <summary>
        /// プレイヤー
        /// </summary>
        private IPlayer _player1;
        private IPlayer _player2;

        /// <summary>
        /// アクティブプレイヤー
        /// </summary>
        public IReadOnlyReactiveProperty<IPlayer> ActivePlayer => _activePlayer;
        private ReactiveProperty<IPlayer> _activePlayer = new(null);

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        private Action _changeNextTurnAction;

        [Inject]
        public PlayerManager(StoneManager stoneManager, PlayerFactory playerFactory, GameSettings gameSettings, PlayerTypeInfoData playerTypeInfoDataData)
        {
            _stoneManager = stoneManager;
            _playerFactory = playerFactory;
            _gameSettings = gameSettings;
            _playerTypeInfoData = playerTypeInfoDataData;
        }

        public void Dispose()
        {
            // 購読解除
            _activePlayer.Dispose();
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
            if (!_gameSettings.DebugOption.isLearnAgent || _activePlayer.Value == null)
            {
                _player1 = _playerFactory.CreatePlayer(player1Type, StoneState.White, PutStone, _gameSettings.Player1Transform);
                _player2 = _playerFactory.CreatePlayer(player2Type, StoneState.Black, PutStone, _gameSettings.Player2Transform);

                // 初期化
                _player1.OnInitialize(player1Type, _gameSettings.DebugOption.isDisplayAnimation);
                _player2.OnInitialize(player2Type, _gameSettings.DebugOption.isDisplayAnimation);

                // デバッグ情報の設定
                _player1.IsWaitSelect = _gameSettings.DebugOption.isWaitSelectStone;
                _player2.IsWaitSelect = _gameSettings.DebugOption.isWaitSelectStone;
            }

            // nullに設定
            _activePlayer.Value = null;
        }

        public void StartGame()
        {
            // プレイヤー1をアクティブプレイヤーに設定
            _activePlayer.Value = _player1;
        }

        public void EndGame()
        {
            // アクティブプレイヤーを初期化
            // 学習中の場合には破棄しない
            if (!_gameSettings.DebugOption.isLearnAgent)
            {
                _activePlayer.Value = null;
            }

            // プレイヤーを破棄する
            var player1ResultState = GetPlayer1ResultState();
            var player2ResultState = GetPlayer2ResultState();
            _player1.OnEndGame(player1ResultState);
            _player2.OnEndGame(player2ResultState);

            // 結果アニメーションを再生
            if (_gameSettings.DebugOption.isDisplayAnimation)
            {
                _player1.StartResultAnimation(player1ResultState);
                _player2.StartResultAnimation(player2ResultState);
            }
        }

        public void DestroyPlayer()
        {
            // 学習中の場合には破棄しない
            if (!_gameSettings.DebugOption.isLearnAgent)
            {
                _player1.OnDestroy();
                _player2.OnDestroy();
            }
        }

        /// <summary>
        /// ターン開始処理
        /// </summary>
        public void StartTurn()
        {
            // 入力プレイヤーの場合、置けるストーンをフォーカスする
            if (_activePlayer.Value.IsInputPlayer()) _stoneManager.SetFocusAllCanPutStones(_activePlayer.Value.MyStoneState);
            // ターン開始
            _activePlayer.Value.OnStartTurn(_stoneManager.GetStoneStatesClone());
        }

        /// <summary>
        /// ターン更新処理
        /// </summary>
        public void UpdateTurn()
        {
            if (_activePlayer.Value == null) return;
            // ターン更新
            _activePlayer.Value.OnUpdateTurn();
        }

        /// <summary>
        /// ターン終了処理
        /// </summary>
        public void EndTurn()
        {
            // アクティブプレイヤーを初期化
            _activePlayer.Value = null;
        }

        /// <summary>
        /// プレイヤー切り替え
        /// </summary>
        /// <param name="turnCount">ターン数</param>
        public void ChangePlayer(int turnCount)
        {
            var isFirstTurn = turnCount % 2 == 0;
            _activePlayer.Value = isFirstTurn ? _player1 : _player2;
        }

        /// <summary>
        /// アクティブプレイヤーがストーンを置けるかどうか？
        /// </summary>
        /// <returns></returns>
        public bool IsCanPutStoneActivePlayer()
        {
            return _stoneManager.GetAllCanPutStonesIndex(_activePlayer.Value.MyStoneState).Count > 0;
        }

        /// <summary>
        /// プレイヤー名を返却する
        /// </summary>
        public string GetPlayer1Name()
        {
            var playerName = _playerTypeInfoData.GetPlayerTypeInfo(_player1.MyPlayerType)?.name;
            return playerName ?? "";
        }
        public string GetPlayer2Name()
        {
            var playerName = _playerTypeInfoData.GetPlayerTypeInfo(_player2.MyPlayerType)?.name;
            return playerName ?? "";
        }

        /// <summary>
        /// プレイヤー種別を返却する
        /// </summary>
        public PlayerType GetPlayer1Type()
        {
            return _player1.MyPlayerType;
        }
        public PlayerType GetPlayer2Type()
        {
            return _player2.MyPlayerType;
        }

        /// <summary>
        /// プレイヤーの結果を返却する
        /// </summary>
        public PlayerResultState GetPlayer1ResultState()
        {
            return _stoneManager.GetPlayerResultState(_player1.MyStoneState);
        }
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
                // ストーン比率からエモーションを設定
                _player1.SetEmotionParameter(_stoneManager.GetStoneStateRate(_player1.MyStoneState));
                _player2.SetEmotionParameter(_stoneManager.GetStoneStateRate(_player2.MyStoneState));

                // ストーンを置くアニメーションを再生
                if (_gameSettings.DebugOption.isDisplayAnimation)
                {
                    _activePlayer.Value.StartPutAnimation();
                }

                // フォーカス解除
                _stoneManager.ReleaseFocusStones();

                // ターン切り替え
                _changeNextTurnAction();
            }
        }
    }
}
