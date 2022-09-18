using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    public class TitleBackView : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        /// <summary>
        /// 戻るボタン
        /// </summary>
        [SerializeField] private Button backButton;
        public void SetActiveBackButton(bool isActive)
        {
            backButton.gameObject.SetActive(isActive);
        }
        public void SetListenerBackButton(UnityAction action)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(action);
        }
    }
}
