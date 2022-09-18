using Reversi.Extensions;
using Reversi.Managers;
using Reversi.UIs.View;
using TMPro;
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
        [Inject] private StoneManager _stoneManager;
        [Inject] private PlayerManager _playerManager;
        [Inject] private GameManager _gameManager;
        [SerializeField] private GameBackView gameBackView;
        [SerializeField] private TextMeshProUGUI blackCountText;
        [SerializeField] private TextMeshProUGUI whiteCountText;
        [SerializeField] private TextMeshProUGUI resultText;

        private void Start()
        {
            blackCountText.SetTextForDynamic("0");;
            whiteCountText.SetTextForDynamic("0");
            resultText.text = "";

            // ストーン数表示
            _stoneManager
                .BlackStoneCount
                .Subscribe(x => blackCountText.SetTextForDynamic(x.ToString()))
                .AddTo(this);
            _stoneManager
                .WhiteStoneCount
                .Subscribe(x => whiteCountText.SetTextForDynamic(x.ToString()))
                .AddTo(this);

            // 戻るボタン
            gameBackView.SetActive(true);
            gameBackView.SetListenerBackButton(() => _gameManager.ChangeTitleScene());

            // 結果表示
            _gameManager
                .State
                .Subscribe(x =>
                {
                    switch (x)
                    {
                        case GameState.Result:
                            var playerResultState = _playerManager.GetPlayer1ResultState();
                            switch (playerResultState)
                            {
                                case PlayerResultState.Win:
                                    resultText.text = "WIN!!";
                                    break;
                                case PlayerResultState.Lose:
                                    resultText.text = "LOSE!!";;
                                    break;
                                default:
                                    resultText.text = "DRAW...";
                                    break;
                            }
                            break;
                        default:
                            resultText.text = "";
                            break;
                    }
                }).AddTo(this);
        }
    }
}
