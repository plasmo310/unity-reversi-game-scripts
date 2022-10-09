using DG.Tweening;
using Reversi.Extensions;
using Reversi.Managers;
using Reversi.Stones.Stone;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    public class GameInfoView : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// アクティブ状態のプレイヤーに応じて色を変える
        /// </summary>
        /// <param name="activeStoneState"></param>
        public void ChangeActiveColor(StoneState activeStoneState)
        {
            whitePlayerInfo.SetActiveColor(false);
            blackPlayerInfo.SetActiveColor(false);

            if (activeStoneState == StoneState.White)
            {
                whitePlayerInfo.SetActiveColor(true);
            }
            else if (activeStoneState == StoneState.Black)
            {
                blackPlayerInfo.SetActiveColor(true);
            }
        }

        /// <summary>
        /// プレイヤー情報
        /// </summary>
        [SerializeField] private GamePlayerInfo whitePlayerInfo;
        public void SetWhitePlayerInfo(string playerName, int count)
        {
            whitePlayerInfo.SetPlayerName(playerName);
            whitePlayerInfo.SetStoneCount(count);
            whitePlayerInfo.SetActiveColor(false);
        }
        public void SetWhiteCount(int count)
        {
            whitePlayerInfo.SetStoneCount(count);
        }
        [SerializeField] private GamePlayerInfo blackPlayerInfo;
        public void SetBlackPlayerInfo(string playerName, int count)
        {
            blackPlayerInfo.SetPlayerName(playerName);
            blackPlayerInfo.SetStoneCount(count);
            blackPlayerInfo.SetActiveColor(false);
        }
        public void SetBlackCount(int count)
        {
            blackPlayerInfo.SetStoneCount(count);
        }

        /// <summary>
        /// プレイヤー結果テキスト
        /// </summary>
        [SerializeField] private TextMeshProUGUI whiteResultText;
        [SerializeField] private TextMeshProUGUI blackResultText;
        public void SetActivePlayerResultText(bool isActive)
        {
            whiteResultText.gameObject.SetActive(isActive);
            blackResultText.gameObject.SetActive(isActive);
        }
        public void ShowPlayerResultText(PlayerResultState whitePlayerResultState, PlayerResultState blackPlayerResultState)
        {
            ShowFeedResultText(whiteResultText, whitePlayerResultState);
            ShowFeedResultText(blackResultText, blackPlayerResultState);
        }
        private void ShowFeedResultText(TextMeshProUGUI text, PlayerResultState playerResultState)
        {
            // メッセージを設定
            var message = "";
            switch (playerResultState)
            {
                case PlayerResultState.Win:
                    message = "WIN!";
                    text.color = new Color(255f/255f, 51f/255f, 51f/255f); // 赤
                    break;
                case PlayerResultState.Lose:
                    message = "LOSE";
                    text.color = new Color(51f/255f, 153f/255f, 255f/255f); // 青
                    break;
                case PlayerResultState.Draw:
                    message = "DRAW";
                    text.color = new Color(102f/255f, 204f/255f, 102f/255f); // 緑
                    break;
            }
            text.text = message;

            var sequence = DOTween.Sequence();
            sequence.AppendCallback(() =>
            {
                text.gameObject.SetActive(true);
            });
            sequence.Append(DOTween.ToAlpha(
                () => text.color,
                color => text.color = color,
                1.0f,
                0.5f
            ));
        }

        /// <summary>
        /// 結果表示
        /// </summary>
        [SerializeField] private GameObject messageArea;
        [SerializeField] private Image messageBackgroundImage;
        [SerializeField] private TextMeshProUGUI messageText;
        public void SetActiveMessageArea(bool isActive)
        {
            messageArea.SetActive(isActive);
        }
        public void ShowMessage(GameModeType gameModeType, PlayerResultState player1ResultState, UnityAction callback)
        {
            var result = "";
            // 観戦モード
            if (gameModeType == GameModeType.WatchPlay)
            {
                result = "終了";
            }
            // VSモード
            else
            {
                switch (player1ResultState)
                {
                    case PlayerResultState.Win:
                        result = "勝利!!";
                        break;
                    case PlayerResultState.Lose:
                        result = "敗北...";;
                        break;
                    default:
                        result = "引き分け";
                        break;
                }
            }
            ShowMessage(result, callback); // 非表示にはせずに表示
        }
        public void ShowMessage(string text, UnityAction callback, float displayDelayTime = 0.0f)
        {
            // テキスト設定
            messageText.SetTextForDynamic(text);

            // 背景がスライドしてテキストが浮かび上がる
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(displayDelayTime);
            sequence.AppendCallback(() =>
            {
                // 最初は非表示
                messageBackgroundImage.fillAmount = 0.0f;
                var c = messageText.color;
                c.a = 0.0f;
                messageText.color = c;
                // オブジェクトを表示する
                SetActiveMessageArea(true);
            });
            sequence.Append(DOTween.To(
                () => messageBackgroundImage.fillAmount,
                (x) => messageBackgroundImage.fillAmount = x,
                1.0f,
                0.5f
            ));
            sequence.Append(DOTween.ToAlpha(
                () => messageText.color,
                color => messageText.color = color,
                1.0f,
                0.2f
                ));

            // コールバックが設定されている場合
            if (callback != null)
            {
                sequence.AppendInterval(1.0f);
                sequence.AppendCallback(() =>
                {
                    callback();
                });
            }
        }

        /// <summary>
        /// スクリーン全体に表示するボタン
        /// </summary>
        [SerializeField] private Button screenNextButton;
        public void SetActiveScreenNextButton(bool isActive)
        {
            screenNextButton.gameObject.SetActive(isActive);
        }
        public void SetListenerScreenNextButton(UnityAction action)
        {
            screenNextButton.onClick.RemoveAllListeners();
            screenNextButton.onClick.AddListener(action);
        }
    }
}
