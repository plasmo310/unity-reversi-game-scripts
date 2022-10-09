using System.Collections.Generic;
using Reversi.Audio;
using Reversi.UIs.View;
using UnityEngine.Events;

namespace Reversi.Services.Impl
{
    /// <summary>
    /// ダイアログ
    /// </summary>
    public class DialogService : IDialogService
    {
        private readonly DialogView _dialogView;
        private readonly IAudioService _audioService;
        public DialogService(DialogView dialogView, IAudioService audioService)
        {
            _dialogView = dialogView;
            _audioService = audioService;

            // 最初は非表示
            _dialogView.SetActive(false);
        }

        /// <summary>
        /// ダイアログ表示
        /// </summary>
        public void ShowMessage(string text, UnityAction callback = null)
        {
            ShowMessage(new List<string> { text }, callback);
        }

        /// <summary>
        /// ダイアログ表示（複数テキスト）
        /// </summary>
        /// <param name="textList"></param>
        /// <param name="callback"></param>
        public void ShowMessage(List<string> textList, UnityAction callback = null)
        {
            var textIndex = 0;
            ChangeOkButtonText(textList, textIndex);
            _dialogView.SetDialogMessageText(textList[textIndex]);
            _dialogView.SetListenerOkButton(() =>
            {
                // 残りのテキストリストに応じて再帰的にコールバックを設定する
                textIndex++;
                SetRecursiveListenerOkButton(textList, textIndex, callback);
            });
            _dialogView.OnShow();
        }

        /// <summary>
        /// テキストインデックスに応じて、OKボタンへ再帰的にリスナーを設定する
        /// </summary>
        /// <param name="textList"></param>
        /// <param name="textIndex"></param>
        /// <param name="callback"></param>
        private void SetRecursiveListenerOkButton(List<string> textList, int textIndex, UnityAction callback)
        {
            ChangeOkButtonText(textList, textIndex);

            if (textIndex >= textList.Count)
            {
                // 最後のテキストの場合、コールバックを設定
                if (callback != null)
                {
                    callback();
                }
                else
                {
                    OnDefaultCallback();
                }
            }
            else
            {
                // テキストがまだ残っている場合、表示する
                _audioService.PlayOneShot(ReversiAudioType.SeClick);
                _dialogView.SetDialogMessageText(textList[textIndex]);

                // 次のListenerを登録する
                _dialogView.SetListenerOkButton(() =>
                {
                    textIndex++;
                    SetRecursiveListenerOkButton(textList, textIndex, callback);
                });
            }
        }

        private void ChangeOkButtonText(List<string> textList, int textIndex)
        {
            var text = textIndex >= textList.Count - 1 ? "OK" : "次へ";
            _dialogView.SetTextOkButtonText(text);
        }

        private void OnDefaultCallback()
        {
            _audioService.PlayOneShot(ReversiAudioType.SeClick);
            _dialogView.OnHide();
        }

        public void HideMessage()
        {
            _dialogView.OnHide();
        }
    }
}
