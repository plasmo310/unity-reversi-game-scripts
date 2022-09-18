using Reversi.Data;
using Reversi.Managers;
using Reversi.Players;
using Reversi.Players.Display;
using Reversi.UIs.View;
using UniRx;
using UnityEngine;
using VContainer;

namespace Reversi.UIs.Presenter
{
    public class TitlePresenter : MonoBehaviour
    {
        [Inject] private TitleManager _titleManager;
        [Inject] private TitleCameraManager _titleCameraManager;
        [Inject] private PlayerTypeInfoData _playerTypeInfoData;
        [Inject] private PlayerSelectBehaviour _playerSelectBehaviour;
        [SerializeField] private TitleBackView titleBackView;
        [SerializeField] private TitleSelectModeView titleSelectModeView;
        [SerializeField] private TitleSelectPlayerView titleSelectPlayerView;
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
                    ShowActiveStateView(state);
                }).AddTo(this);
        }

        private void OnInitializeAllView(TitleState state)
        {
            // UIを全て非表示
            HideAllView();

            // ボタンイベントを設定して画面を表示
            SetAllButtonListener();
            ShowActiveStateView(state);
        }

        private void HideAllView()
        {
            titleBackView.SetActive(false);
            titleSelectModeView.SetActive(false);
            titleSelectPlayerView.SetActive(false);
        }

        private void ShowActiveStateView(TitleState state)
        {
            // 状態に応じたViewを表示する
            switch (state)
            {
                case TitleState.SelectMode:
                    titleSelectModeView.SetActive(true);
                    // 戻るボタンを非表示
                    titleBackView.SetActive(false);
                    titleBackView.SetListenerBackButton(null);
                    break;
                case TitleState.SelectPlayer:
                    ResetSelectPlayer();
                    titleSelectPlayerView.SetActive(true);
                    // 戻るボタンを表示
                    titleBackView.SetActive(true);
                    titleBackView.SetListenerBackButton(() =>
                    {
                        _playerSelectBehaviour.HideAllSelectPlayer();
                        _titleManager.ChangeReserveState(TitleState.SelectMode);
                    });
                    break;
            }
        }

        /// <summary>
        /// 選択プレイヤーのリセット
        /// </summary>
        private void ResetSelectPlayer()
        {
            // 選択プレイヤーをリセット
            _selectPlayer = PlayerType.None;
            // Toggleを未選択の状態にして詳細も非表示にする
            titleSelectPlayerView.ResetSelectPlayerToggles();
            titleSelectPlayerView.SetInteractableDecideButton(false);
            titleSelectPlayerView.SetActivePlayerDetailArea(false);
            titleSelectPlayerView.SetPlayerDetailText("", "", "");
        }

        #region ButtonListener

        /// <summary>
        /// ボタンイベント設定
        /// </summary>
        private void SetAllButtonListener()
        {
            SetSelectModeButtonListener();
            SetSelectPlayerButtonListener();
        }

        /// <summary>
        /// モード選択のボタンイベント設定
        /// </summary>
        private void SetSelectModeButtonListener()
        {
            // ゲームモード選択ボタン
            titleSelectModeView.SetListenerSinglePlayButton(() =>
            {
                _playerSelectBehaviour.ShowAllSelectPlayer();
                _titleManager.SetSelectGameMode(GameModeType.SinglePlay);
            });
            titleSelectModeView.SetListenerWatchPlayButton(() =>
            {
                _playerSelectBehaviour.ShowAllSelectPlayer();
                _titleManager.SetSelectGameMode(GameModeType.WatchPlay);
            });
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
                titleSelectPlayerView.SetInteractableDecideButton(true);
                titleSelectPlayerView.SetActivePlayerDetailArea(true);

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
                titleSelectPlayerView.SetInteractableDecideButton(false);

                // 選択したプレイヤーを渡す
                var isAllSelected = _titleManager.SetSelectPlayer(_selectPlayer);

                // 全てのプレイヤーを選択していない場合、選択状態をリセットする
                if (!isAllSelected) ResetSelectPlayer();
            });
        }

        #endregion
    }
}
