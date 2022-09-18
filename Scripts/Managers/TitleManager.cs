using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Common;
using Reversi.Players;
using Reversi.Settings;
using UniRx;

namespace Reversi.Managers
{
    /// <summary>
    /// タイトル画面管理クラス
    /// </summary>
    public class TitleManager : IDisposable
    {
        private readonly GameSettings _gameSettings;
        private readonly TitleCameraManager _titleCameraManager;

        /// <summary>
        /// 状態
        /// </summary>
        private readonly ReactiveProperty<TitleState> _state = new(TitleState.SelectMode);
        public IReadOnlyReactiveProperty<TitleState> State => _state;
        private readonly StateMachine<TitleManager> _stateMachine;

        public TitleManager(GameSettings gameSettings, TitleCameraManager titleCameraManager)
        {
            _gameSettings = gameSettings;
            _titleCameraManager = titleCameraManager;

            // ステートマシン設定
            _stateMachine = new StateMachine<TitleManager>(this);
            _stateMachine.SetChangeStateEvent(ChangeStateEvent);
            _stateMachine.Add<StateSelectMode>((int) TitleState.SelectMode);
            _stateMachine.Add<StateSelectPlayer>((int) TitleState.SelectPlayer);
        }

        public void OnStart()
        {
            _stateMachine.OnStart((int) TitleState.SelectMode);
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

        private void ChangeStateEvent(int stateId)
        {
            // Stateを設定
            var state = (TitleState) Enum.ToObject(typeof(TitleState), stateId);
            _state.Value = state;

            // カメラを切り替える
            _titleCameraManager.ChangeCamera(state);
        }

        /// <summary>
        /// 遷移先のステートを予約する
        /// </summary>
        private TitleState _reserveState = TitleState.None;
        public void ChangeReserveState(TitleState state)
        {
            _reserveState = state;
        }

        // ----- UI側から呼ばれる -----

        /// <summary>
        /// 選択したモードを設定する
        /// </summary>
        /// <param name="gameModeType"></param>
        public void SetSelectGameMode(GameModeType gameModeType)
        {
            _gameSettings.SelectGameModeType = gameModeType;
        }

        /// <summary>
        /// 選択したプレイヤーを設定する
        /// </summary>
        /// <returns>全てのプレイヤーの設定が完了したか？</returns>
        public bool SetSelectPlayer(PlayerType player)
        {
            // プレイヤー1の設定
            if (_gameSettings.SelectPlayer1 == PlayerType.None)
            {
                _gameSettings.SelectPlayer1 = player;
                return false;
            }
            // プレイヤー2の設定
            _gameSettings.SelectPlayer2 = player;
            return true;
        }

        // ----- モード選択 -----
        private class StateSelectMode : StateMachine<TitleManager>.StateBase
        {
            private CancellationTokenSource _cancellationTokenSource;
            public override void OnStart()
            {
                // モード選択開始
                _cancellationTokenSource = new CancellationTokenSource();
                StartSelectModeAsync(_cancellationTokenSource.Token);
            }

            public override void OnUpdate()
            {
                // 次のステートが予約された場合
                if (Owner._reserveState != TitleState.None)
                {
                    // タスクがあればキャンセル
                    _cancellationTokenSource?.Cancel();
                    // 次のステートに遷移
                    StateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = TitleState.None;
                }
            }

            private async void StartSelectModeAsync(CancellationToken token)
            {
                // 選択モードを初期化
                Owner._gameSettings.SelectGameModeType = GameModeType.None;

                // UI側で選択されるまで待つ
                await UniTask.WaitWhile(() =>
                        Owner._gameSettings.SelectGameModeType == GameModeType.None,
                    cancellationToken: token);

                // 選択されたらプレイヤー選択に遷移
                StateMachine.ChangeState((int) TitleState.SelectPlayer);
            }
        }

        // ----- プレイヤー選択 -----
        private class StateSelectPlayer : StateMachine<TitleManager>.StateBase
        {
            private CancellationTokenSource _cancellationTokenSource;
            public override void OnStart()
            {
                // プレイヤー選択開始
                _cancellationTokenSource = new CancellationTokenSource();
                StartSelectPlayersAsync(_cancellationTokenSource.Token);
            }

            public override void OnUpdate()
            {
                // 次のステートが予約された場合
                if (Owner._reserveState != TitleState.None)
                {
                    // タスクがあればキャンセル
                    _cancellationTokenSource?.Cancel();
                    // 次のステートに遷移
                    StateMachine.ChangeState((int) Owner._reserveState);
                    Owner._reserveState = TitleState.None;
                }
            }

            private async void StartSelectPlayersAsync(CancellationToken token)
            {
                // 選択プレイヤーを初期化
                Owner._gameSettings.SelectPlayer1 = Owner._gameSettings.SelectGameModeType == GameModeType.SinglePlay
                    ? PlayerType.InputPlayer // SinglePlayの場合は入力プレイヤーを設定
                    : PlayerType.None;
                Owner._gameSettings.SelectPlayer2 = PlayerType.None;

                // UI側で選択されるまで待つ
                await UniTask.WaitWhile(() =>
                    Owner._gameSettings.SelectPlayer1 == PlayerType.None ||
                    Owner._gameSettings.SelectPlayer2 == PlayerType.None,
                    cancellationToken: token);

                // 選択されたらゲーム開始
                SceneLoader.LoadScene("GameScene");
            }
        }
    }
}
