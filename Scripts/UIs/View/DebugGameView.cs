using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    public class DebugGameView : MonoBehaviour
    {
        [SerializeField] private Button winButton;
        public void SetListenerWinButton(UnityAction action)
        {
            winButton.onClick.RemoveAllListeners();
            winButton.onClick.AddListener(action);
        }

        [SerializeField] private Button loseButton;
        public void SetListenerLoseButton(UnityAction action)
        {
            loseButton.onClick.RemoveAllListeners();
            loseButton.onClick.AddListener(action);
        }

        [SerializeField] private Button drawButton;
        public void SetListenerDrawButton(UnityAction action)
        {
            drawButton.onClick.RemoveAllListeners();
            drawButton.onClick.AddListener(action);
        }
    }
}
