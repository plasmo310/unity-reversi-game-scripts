using System.Collections.Generic;
using Reversi.Audio;
using Reversi.Data;
using Reversi.DB;
using Reversi.Managers;
using Reversi.Players;
using Reversi.Players.Display;
using Reversi.Services;
using Reversi.Settings;
using Reversi.UIs.View;
using UniRx;
using UnityEngine;
using VContainer;

namespace Reversi.UIs.Presenter
{
    public class TitlePresenter : MonoBehaviour
    {
        [SerializeField] private TitleBackView titleBackView;
        [SerializeField] private TitleTopView titleTopView;
        [SerializeField] private TitleSelectModeView titleSelectModeView;
        [SerializeField] private TitleSelectPlayerView titleSelectPlayerView;

        private TitleManager _titleManager;
        private TitleCameraManager _titleCameraManager;
        private PlayerTypeInfoData _playerTypeInfoData;
        private PlayerSelectBehaviour _playerSelectBehaviour;
        private PlayerPrefsRepository _playerPrefsRepository;
        private IAudioService _audioService;
        private IDialogService _dialogService;
        private GameSettings _gameSettings;

        [Inject]
        public void Construct(TitleManager titleManager, TitleCameraManager titleCameraManager, PlayerTypeInfoData playerTypeInfoData, PlayerSelectBehaviour playerSelectBehaviour, PlayerPrefsRepository playerPrefsRepository, IAudioService audioService, IDialogService dialogService, GameSettings gameSettings)
        {
            _titleManager = titleManager;
            _titleCameraManager = titleCameraManager;
            _playerTypeInfoData = playerTypeInfoData;
            _playerSelectBehaviour = playerSelectBehaviour;
            _playerPrefsRepository = playerPrefsRepository;
            _audioService = audioService;
            _dialogService = dialogService;
            _gameSettings = gameSettings;
        }

        private PlayerType _selectPlayer;

        private void Awake()
        {
            // UI初期化
            OnInitializeAllView(_titleManager.State.Value);
        }

        private void Start()
        {
            // タイトル画面の状態変更検知
            _titleManager
                .State
                .Subscribe(state =>
                {
                    // Viewを全て非表示にする
                    HideAllView();

                    // 状態に応じたViewを表示する
                    OnShowActiveStateView(state);
                }).AddTo(this);
        }

        /// <summary>
        /// 全てのView初期化
        /// </summary>
        /// <param name="state"></param>
        private void OnInitializeAllView(TitleState state)
        {
            // UIを全て非表示
            HideAllView();

            // 各Viewの初期化
            OnInitializeTitleTopView();
            OnInitializeSelectPlayerView();

            // ボタンイベントを設定して画面を表示
            OnSetAllButtonListener();
            OnShowActiveStateView(state);
        }

        /// <summary>
        /// 全てのViewの非表示
        /// </summary>
        private void HideAllView()
        {
            titleBackView.SetActive(false);
            titleTopView.SetActive(false);
            titleSelectModeView.SetActive(false);
            titleSelectPlayerView.SetActive(false);
        }

        /// <summary>
        /// ステートに応じたViewの表示
        /// </summary>
        private void OnShowActiveStateView(TitleState state)
        {
            // 状態に応じたViewを表示する
            switch (state)
            {
                case TitleState.SelectMode:
                    OnShowSelectModeButtonView();
                    break;
                case TitleState.SelectPlayer:
                    OnShowSelectPlayerView();
                    break;
            }
        }

        /// <summary>
        /// ボタンイベント設定
        /// </summary>
        private void OnSetAllButtonListener()
        {
            SetTitleTopButtonListener();
            SetSelectModeButtonListener();
            SetSelectPlayerButtonListener();
        }

        #region タイトルTOP画面

        /// <summary>
        /// タイトルTOP画面初期化
        /// </summary>
        private void OnInitializeTitleTopView()
        {
            // ボスを撃破済であればオセロ戦士バッジを表示する
            var isDefeatLastBoss = _playerPrefsRepository.IsDefeatLastBoss();
            titleTopView.SetActiveReversiMasterBadge(isDefeatLastBoss);
        }

        /// <summary>
        /// タイトルTOP画面のボタンイベント設定
        /// </summary>
        private void SetTitleTopButtonListener()
        {
            // MORE APPボタン
            titleTopView.SetListenerMoreAppButton(() =>
            {
                _audioService.PlayOneShot(ReversiAudioType.SeClick);
#if UNITY_ANDROID
                Application.OpenURL("https://play.google.com/store/apps/developer?id=MOLEGORO");
#elif UNITY_IPHONE
                Application.OpenURL("https://apps.apple.com/jp/developer/masato-watanabe/id1523138920");
#else
                Application.OpenURL("https://elekibear.com/molegoro_app");
#endif
            });

            // ヘルプボタン
            titleTopView.SetListenerHelpButton(() =>
            {
                _audioService.PlayOneShot(ReversiAudioType.SeClick);
                _dialogService.ShowMessage("ゆるいクラスメイト達と<br>オセロして楽しみましょう!<br><br>勝ち続けると最強の戦士が現れるかも...?");
            });

            // SEボタン
            titleTopView.SetListenerSeButton(() =>
            {
                var isVolumeOff = _audioService.ChangeSeVolumeOnOff();
                titleTopView.ChangeSeOnOffSprite(isVolumeOff);
                _playerPrefsRepository.SaveIsSeVolumeOff(isVolumeOff);
            });
            titleTopView.ChangeSeOnOffSprite(_playerPrefsRepository.GetIsSeVolumeOff());

            // BGMボタン
            titleTopView.SetListenerBgmButton(() =>
            {
                var isVolumeOff = _audioService.ChangeBgmVolumeOnOff();
                titleTopView.ChangeBgmOnOffSprite(isVolumeOff);
                _playerPrefsRepository.SaveIsBgmVolumeOff(isVolumeOff);
            });
            titleTopView.ChangeBgmOnOffSprite(_playerPrefsRepository.GetIsBgmVolumeOff());
        }

        #endregion

        #region モード選択画面

        /// <summary>
        /// モード選択画面表示
        /// </summary>
        private void OnShowSelectModeButtonView()
        {
            titleSelectModeView.SetActive(true);
            titleTopView.SetActive(true);
            // 戻るボタンを非表示
            titleBackView.SetActive(false);
            titleBackView.SetListenerBackButton(null);
        }

        /// <summary>
        /// モード選択のボタンイベント設定
        /// </summary>
        private void SetSelectModeButtonListener()
        {
            // ゲームモード選択ボタン
            titleSelectModeView.SetListenerSinglePlayButton(() =>
            {
                _audioService.PlayOneShot(ReversiAudioType.SeClick);
                _playerSelectBehaviour.ShowAllSelectPlayer();
                _titleManager.SetSelectGameMode(GameModeType.SinglePlay);
            });
            titleSelectModeView.SetListenerWatchPlayButton(() =>
            {
                _audioService.PlayOneShot(ReversiAudioType.SeClick);
                _playerSelectBehaviour.ShowAllSelectPlayer();
                _titleManager.SetSelectGameMode(GameModeType.WatchPlay);
            });
        }

        #endregion

        #region プレイヤー選択画面

        /// <summary>
        /// プレイヤー選択画面初期化
        /// </summary>
        private void OnInitializeSelectPlayerView()
        {
            // 解放済プレイヤーを取得
            var releasePlayerTypes = _playerPrefsRepository.GetReleasePlayers();
            var isReleaseBoss = releasePlayerTypes.Contains(PlayerType.ZeroPlayer); // ゼロを倒しているかどうか？

            // 生成するプレイヤー情報の設定
            var createPlayerTypes = new List<PlayerType>();
            createPlayerTypes.Add(PlayerType.None);
            createPlayerTypes.Add(PlayerType.PikaruPlayer);
            createPlayerTypes.Add(PlayerType.MichaelPlayer);
            createPlayerTypes.Add(PlayerType.ElekiBearPlayer);
            createPlayerTypes.Add(PlayerType.GoloyanPlayer);
            createPlayerTypes.Add(PlayerType.ZeroPlayer);
            // シミュレーションAIは解放されるまで非表示にする
            var noDisplayUntilReleasePlayerTypes = new List<PlayerType>
            {
                PlayerType.MiniMaxAIRobotPlayer,
                PlayerType.MonteCarloAIRobotPlayer,
                PlayerType.MlAgentAIRobotPlayer
            };
            foreach (var noDisplayUntilReleasePlayerType in noDisplayUntilReleasePlayerTypes)
            {
                if (releasePlayerTypes.Contains(noDisplayUntilReleasePlayerType))
                {
                    createPlayerTypes.Add(noDisplayUntilReleasePlayerType);
                }
            }

            // プレイヤー選択Toggleを生成する
            foreach (var playerType in createPlayerTypes)
            {
                var isRelease = releasePlayerTypes.Contains(playerType);
                var playerTypeData = _playerTypeInfoData.GetPlayerTypeInfo(playerType);
                titleSelectPlayerView.CreatePlayerToggle(playerType, playerTypeData.iconSprite, playerTypeData.imageColor, playerTypeData.optionalLabel, isRelease);
            }

            // 表示するプレイヤーを設定
            foreach (var releasePlayerType in releasePlayerTypes)
            {
                _playerSelectBehaviour.SetShowTargetPlayerType(releasePlayerType);
            }
            // 最初は全て非表示
            _playerSelectBehaviour.HideAllSelectPlayer();

            // ボス(ゼロ)を倒している場合、解放状況によってパンアニメーションの有効/無効を切り替える
            if (isReleaseBoss)
            {
                _titleCameraManager.SetIsEnableAllPlayerCameraPanAnimation(true);
            }

            // 解放したキャラクターが3種類以上の場合、観戦モードを有効にする
            var isInteractableWatchMode = releasePlayerTypes.Count >= 3;
            titleSelectModeView.SetIsInteractableWatchPlayerButton(isInteractableWatchMode);
        }

        /// <summary>
        /// プレイヤー選択画面表示
        /// </summary>
        private void OnShowSelectPlayerView()
        {
            ResetSelectPlayer(_gameSettings.SelectGameModeType);
            titleSelectPlayerView.SetActive(true);
            // 戻るボタンを表示
            titleBackView.SetActive(true);
            titleBackView.SetListenerBackButton(() =>
            {
                _audioService.PlayOneShot(ReversiAudioType.SeClick);
                _playerSelectBehaviour.HideAllSelectPlayer();
                _titleManager.ChangeReserveState(TitleState.SelectMode);
            });
        }

        /// <summary>
        /// 選択プレイヤーのリセット
        /// </summary>
        private void ResetSelectPlayer(GameModeType gameModeType, bool isSecondDisplay = false)
        {
            // Toggleを未選択の状態にして詳細も非表示にする
            titleSelectPlayerView.ResetSelectPlayerToggles();
            titleSelectPlayerView.SetActiveDecideButton(false);
            titleSelectPlayerView.SetActivePlayerDetailArea(false);
            titleSelectPlayerView.SetPlayerDetailText("", "", "");
            titleSelectPlayerView.SetTextSelectMessageText(GetSelectPlayerMessageText(gameModeType, isSecondDisplay));
            titleSelectPlayerView.SetActiveSelectMessageText(true);
        }

        /// <summary>
        /// 選択メッセージを返却する
        /// </summary>
        private string GetSelectPlayerMessageText(GameModeType gameModeType, bool isSecondDisplay = false)
        {
            // VSモード
            if (gameModeType == GameModeType.SinglePlay)
            {
                return "CHARACTER SELECT";
            }
            // 観戦モード
            return !isSecondDisplay ? "PLAYER1 SELECT" : "PLAYER2 SELECT";
        }

        /// <summary>
        /// プレイヤー選択のボタンイベント設定
        /// </summary>
        private void SetSelectPlayerButtonListener()
        {
            // プレイヤー選択Toggle
            titleSelectPlayerView.SetListenerSelectPlayerToggles((isOn, selectPlayerType) =>
            {
                if (!isOn) return;
                if (_selectPlayer == selectPlayerType) return;

                if (selectPlayerType != PlayerType.None)
                {
                    _audioService.PlayOneShot(ReversiAudioType.SeClick);
                }

                titleSelectPlayerView.SetActiveDecideButton(true);
                titleSelectPlayerView.SetActivePlayerDetailArea(true);
                titleSelectPlayerView.SetActiveSelectMessageText(false);

                // 選択したプレイヤーを表示する
                _playerSelectBehaviour.ShowSelectPlayer(selectPlayerType);

                // 選択したプレイヤータイプから名前と詳細を取得、設定する
                var playerTypeInfo = _playerTypeInfoData.GetPlayerTypeInfo(selectPlayerType);
                titleSelectPlayerView.SetPlayerDetailText(playerTypeInfo.name, playerTypeInfo.studentNo, playerTypeInfo.detail);

                // 選択したプレイヤーを設定
                _selectPlayer = selectPlayerType;

                // カメラも切り替える
                _titleCameraManager.ChangeSelectPlayerCamera(selectPlayerType);
            });

            // 決定ボタン
            titleSelectPlayerView.SetListenerDecideButton(() =>
            {

                // ボタンを非活性にする
                titleSelectPlayerView.SetActiveDecideButton(false);

                // 選択したプレイヤーを渡す
                var isAllSelected = _titleManager.SetSelectPlayer(_selectPlayer);

                // 全てのプレイヤーを選んだ場合にはSEを変える
                var audioType = isAllSelected ? ReversiAudioType.SeDecide : ReversiAudioType.SeClick;
                _audioService.PlayOneShot(audioType);

                // 全てのプレイヤーを選択していない場合、選択状態をリセットする
                if (!isAllSelected) ResetSelectPlayer(_gameSettings.SelectGameModeType, true);
            });
        }
        #endregion
    }
}
