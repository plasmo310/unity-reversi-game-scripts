using System.Collections.Generic;
using UnityEngine.Events;

namespace Reversi.Services
{
    public interface IDialogService
    {
        public void ShowMessage(string text, UnityAction callback = null);
        public void ShowMessage(List<string> textList, UnityAction callback = null);
        public void HideMessage();
    }
}
