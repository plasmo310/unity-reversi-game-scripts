using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Audio;
using Reversi.Common;
using Reversi.Const;
using Reversi.Players;
using Reversi.Services;
using Reversi.Settings;
using UniRx;
using VContainer;

namespace Reversi.Managers
{
    /// <summary>
    /// ゲーム画面管理クラス
    /// </summary>
    public class GameManager : IDisposable
    {
        private readonly StoneManager _stoneManager;
        private readonly PlayerManager _playerManager;
        private readonly GameSettings _gameSettings;
        private readonly ILogService _logService;
        private readonly IAudioService _audioService;
        private readonly ITransitionService _transitionService;

        /// <summary>
        /// 選択プレイヤー
        /// </summary>
        private PlayerType _selectPlayer1Type;
        private PlayerType _selectPlayer2Type;

        /// <summary>
        /// ゲーム状態
        /// </summary>
        private readonly ReactiveProperty<GameState> _state = new(GameState.Play);
        public IReadOnlyReactiveProperty<GameState> State => _state;
        private readonly StateMachine<GameManager> _stateMachine;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        [Inject]
        public GameManager(StoneManager stoneManager, PlayerManager playerManager, GameSettings gameSettings, ILogService logService, IAudioService audioService, ITransitionService transitionService)
        {
            _stoneManager = stoneManager;
            _playerManager = playerManager;
            _gameSettings = gameSettings;
            _logService = logService;
            _audioService = audioService;
            _transitionService = transitionService;

            // ステートマシン設定
            _stateMachine = new StateMachine<GameManager>(this);
            _stateMachine.SetChangeStateEvent((stateId) =>
            {
                // ステート変更時にReactivePropertyにも反映
                _state.Value = (GameState) Enum.ToObject(typeof(GameState), stateId);
            });
            _stateMachine.Add<StatePlay>((int) GameState.Play);
            _stateMachine.Add<StateResult>((int) GameState.Result);

            // BGM開始
            _audioService.PlayBGM(ReversiAudioType.BgmBattle);

            // キャンセルトークン発行
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void OnStart()
        {
            // プレイヤーの設定
            _selectPlayer1Type = _gameSettings.SelectPlayer1;
            _selectPlayer2Type = _gameSettings.SelectPlayer2;

            // ゲーム開始
            _stateMachine.OnStart((int) GameState.Play);
        }

        public void OnUpdate()
        {
            _stateMachine.OnUpdate();
        }

        public void Dispose()
        {
            // 購読解除
            _state.Dispose();
            // キャンセル処理
            _cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// 前のシーン遷移処理
        /// 戻るボタン押下時に呼ばれる
        /// </summary>
        public void OnBackScene()
        {
            ChangeTitleScene();
        }

        /// <summary>
        /// 次のシーン遷移処理
        /// バトル結果表示後にボタン押下で呼ばれる
        /// </summary>
        public void OnNextScene(bool isInterstitial)
        {
            // 遷移時にプレイヤーを破棄
            _playerManager.DestroyPlayer();

            if (isInterstitial)
            {
                ChangeInterstitialScene();
            }
            else
            {
                ChangeTitleScene();
            }
        }

        /// <summary>
        /// シーン遷移処理
        /// </summary>
        private void ChangeTitleScene()
        {
            _transitionService.LoadScene(GameConst.SceneNameTitle);
        }
        private void ChangeInterstitialScene()
        {
            _transitionService.LoadScene(GameConst.SceneNameInterstitial);
        }

        // ----- プレイ中 -----
        private class StatePlay : StateMachine<GameManager>.StateBase
        {
            private int _turnCount = 0;
            private bool _isGameStart = false;
            public override void OnStart()
            {
                _turnCount = 0;
                _isGameStart = false;

                // ストーン初期化
                Owner._stoneManager.InitializeStones();

                // ゲーム初期化してゲーム開始
                Owner._playerManager.InitializeGame(
                    Owner._selectPlayer1Type, Owner._selectPlayer2Type,
                    () => ChangeNextTurnAsync(Owner._cancellationTokenSource.Token));
                StartGameAsync(Owner._cancellationTokenSource.Token);
            }

            private async void StartGameAsync(CancellationToken token)
            {
                // 一定時間待機
                await UniTask.Delay(2300, cancellationToken: token);

                // ゲーム開始
                Owner._playerManager.StartGame();
                Owner._playerManager.StartTurn();
                _isGameStart = true;
            }

            public override void OnUpdate()
            {
                if (!_isGameStart) return;

                // ターン更新
                Owner._playerManager.UpdateTurn();
            }

            /// <summary>
            /// 次のターンに変更する
            /// </summary>
            private async void ChangeNextTurnAsync(CancellationToken token)
            {
                // オセロ対局中は常に監視する
                while (true)
                {
                    _turnCount++;

                    //  全てのストーンが置けない場合、ゲーム終了
                    if (!Owner._stoneManager.IsCanPutStone())
                    {
                        EndGameAsync(Owner._cancellationTokenSource.Token);
                        return;
                    }

                    // ターン終了
                    Owner._playerManager.EndTurn();

                    // アニメーション再生分、少し待機
                    if (Owner._gameSettings.DebugOption.isDisplayAnimation)
                    {
                        await UniTask.Delay(1000, cancellationToken: token);
                    }

                    // プレイヤーの切り替え
                    Owner._playerManager.ChangePlayer(_turnCount);

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
            private async void EndGameAsync(CancellationToken token)
            {
                // BGMを停止させて少し待機
                Owner._audioService.StopBGM();
                await UniTask.Delay(2000, cancellationToken: token);

                // プレイヤーが勝った場合、もしくは観戦モードの場合は勝利のBGMを再生
                var result = Owner._playerManager.GetPlayer1ResultState();
                var playBgm = Owner._gameSettings.SelectGameModeType == GameModeType.WatchPlay || result == PlayerResultState.Win
                    ? ReversiAudioType.BgmBattleWin
                    : ReversiAudioType.BgmBattleLose;
                Owner._audioService.PlayBGM(playBgm);

                // ゲーム終了
                Owner._playerManager.EndGame();
                StateMachine.ChangeState((int)GameState.Result);
            }
        }

        // ----- 結果表示 -----
        private class StateResult : StateMachine<GameManager>.StateBase
        {
            public override void OnStart()
            {
                // ログ出力
                Owner._logService.PrintLog(
                    "player1=>" + Owner._playerManager.GetPlayer1ResultState().ToString() +
                    " black: " + Owner._stoneManager.BlackStoneCount + " white: " + Owner._stoneManager.WhiteStoneCount);

                // ゲームループ指定時はそのまま再度プレイ
                if (Owner._gameSettings.DebugOption.isGameLoop)
                {
                    Owner._playerManager.DestroyPlayer();
                    StateMachine.ChangeState((int) GameState.Play);
                }
            }
        }
    }
}
