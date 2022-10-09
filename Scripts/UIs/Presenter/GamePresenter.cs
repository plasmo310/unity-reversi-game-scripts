using System.Collections.Generic;
using Reversi.Audio;
using Reversi.DB;
using Reversi.Managers;
using Reversi.Players;
using Reversi.Services;
using Reversi.Settings;
using Reversi.Stones.Stone;
using Reversi.UIs.View;
using UniRx;
using UnityEngine;
using VContainer;

namespace Reversi.UIs.Presenter
{
    /// <summary>
    /// GamePresenter
    /// </summary>
    public class GamePresenter : MonoBehaviour
    {
        [SerializeField] private GameBackView gameBackView;
        [SerializeField] private GameInfoView gameInfoView;

        private StoneManager _stoneManager;
        private PlayerManager _playerManager;
        private GameManager _gameManager;
        private IAudioService _audioService;
        private IDialogService _dialogService;
        private PlayerPrefsRepository _playerPrefsRepository;
        private GameSettings _gameSettings;

        [Inject]
        public void Construct(StoneManager stoneManager, PlayerManager playerManager, GameManager gameManager, IAudioService audioService, IDialogService dialogService, PlayerPrefsRepository playerPrefsRepository, GameSettings gameSettings)
        {
            _stoneManager = stoneManager;
            _playerManager = playerManager;
            _gameManager = gameManager;
            _audioService = audioService;
            _dialogService = dialogService;
            _playerPrefsRepository = playerPrefsRepository;
            _gameSettings = gameSettings;
        }

        private void Start()
        {
            OnInitializeAllView();
        }

        /// <summary>
        /// 全てのView初期化
        /// </summary>
        private void OnInitializeAllView()
        {
            // Canvas表示
            gameInfoView.SetActive(true);

            // 初期情報を設定
            gameInfoView.SetWhitePlayerInfo(_playerManager.GetPlayer1Name(), 0);
            gameInfoView.SetBlackPlayerInfo(_playerManager.GetPlayer2Name(), 0);
            gameInfoView.SetActiveMessageArea(false);
            gameInfoView.SetActivePlayerResultText(false);
            gameInfoView.SetActiveScreenNextButton(false);

            // ボタンイベント設定
            OnSetAllButtonListener();

            // ステート監視設定
            OnSetStateSubscribe();
        }

        /// <summary>
        /// ボタンイベント設定
        /// </summary>
        private void OnSetAllButtonListener()
        {
            // 戻るボタン
            gameBackView.SetActive(true);
            gameBackView.SetListenerBackButton(() =>
            {
                _audioService.PlayOneShot(ReversiAudioType.SeClick);
                _gameManager.OnBackScene();
            });
        }

        /// <summary>
        /// ステート監視設定
        /// </summary>
        private void OnSetStateSubscribe()
        {
            // アクティブプレイヤーによる色の変更
            _playerManager
                .ActivePlayer
                .Subscribe(activePlayer =>
                {
                    var activeStoneState = activePlayer?.MyStoneState ?? StoneState.Empty;
                    gameInfoView.ChangeActiveColor(activeStoneState);
                })
                .AddTo(this);

            // ストーン数表示
            _stoneManager
                .WhiteStoneCount
                .Subscribe(count => gameInfoView.SetWhiteCount(count))
                .AddTo(this);
            _stoneManager
                .BlackStoneCount
                .Subscribe(count => gameInfoView.SetBlackCount(count))
                .AddTo(this);

            // ゲーム状態による切替
            _gameManager
                .State
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        // 開始表示
                        case GameState.Play:
                            OnShowStart();
                            break;
                        // 結果表示
                        case GameState.Result:
                            OnShowResult();
                            break;
                    }
                }).AddTo(this);
        }

        /// <summary>
        /// 開始表示
        /// </summary>
        private void OnShowStart()
        {
            gameInfoView.ShowMessage("対局開始", () =>
            {
                gameInfoView.SetActiveMessageArea(false);
            }, 0.5f);
        }

        /// <summary>
        /// リザルト表示
        /// </summary>
        private void OnShowResult()
        {
            // 結果を表示する
            var player1ResultState = _playerManager.GetPlayer1ResultState();
            var player2ResultState = _playerManager.GetPlayer2ResultState();
            gameBackView.SetActive(false);

            // 結果メッセージ表示
            gameInfoView.ShowMessage(_gameSettings.SelectGameModeType, player1ResultState, () =>
            {
                // 画面全体のタップボタンを表示する
                gameInfoView.SetListenerScreenNextButton(() =>
                {
                    // AI解放メッセージ
                    var releaseMessageList = new List<string>();

                    // インタースティシャル広告を表示するか？
                    // 1/5の確率くらいで再生する
                    var isInterstitial = Random.Range(0, 3) == 0;
                    // まだ誰も撃破していない場合は表示しない
                    var maxDefeatPlayerType = _playerPrefsRepository.GetMaxDefeatPlayerType();
                    if (maxDefeatPlayerType == PlayerType.None)
                    {
                        isInterstitial = false;
                    }

                    // VSモードでAIに勝利した場合
                    if (_gameSettings.SelectGameModeType == GameModeType.SinglePlay
                        && player1ResultState == PlayerResultState.Win)
                    {
                        // PlayerPrefsに保存し、更新できた場合には解放メッセージを取得する
                        var defeatPlayerType = _playerManager.GetPlayer2Type();
                        if (_playerPrefsRepository.SaveMaxDefeatPlayerType(defeatPlayerType))
                        {
                            releaseMessageList = _playerPrefsRepository.GetReleasePlayerMessages(defeatPlayerType);
                        }
                    }

                    // AI解放メッセージがある場合、ダイアログを表示してから遷移する
                    if (releaseMessageList != null && releaseMessageList.Count > 0)
                    {
                        _audioService.PlayOneShot(ReversiAudioType.SeClick);
                        _dialogService.ShowMessage(releaseMessageList,
                            () =>
                            {
                                _audioService.PlayOneShot(ReversiAudioType.SeDecide);
                                _dialogService.HideMessage();
                                _gameManager.OnNextScene(isInterstitial);
                            });
                    }
                    else
                    {
                        _audioService.PlayOneShot(ReversiAudioType.SeDecide);
                        _gameManager.OnNextScene(isInterstitial);
                    }
                });
                gameInfoView.SetActiveScreenNextButton(true);
            });

            // プレイヤー情報欄の結果を表示
            gameInfoView.ShowPlayerResultText(player1ResultState, player2ResultState);
        }
    }
}
