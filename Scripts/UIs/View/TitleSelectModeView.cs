using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    /// <summary>
    /// モード選択View
    /// </summary>
    public class TitleSelectModeView : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// モード選択ボタン
        /// </summary>
        [SerializeField] private Button singlePlayButton;
        [SerializeField] private Button watchPlayButton;
        public void SetListenerSinglePlayButton(UnityAction action)
        {
            singlePlayButton.onClick.RemoveAllListeners();
            singlePlayButton.onClick.AddListener(action);
        }
        public void SetListenerWatchPlayButton(UnityAction action)
        {
            watchPlayButton.onClick.RemoveAllListeners();
            watchPlayButton.onClick.AddListener(action);
        }
        public void SetIsInteractableWatchPlayerButton(bool isInteractable)
        {
            watchPlayButton.interactable = isInteractable;
        }
    }
}
