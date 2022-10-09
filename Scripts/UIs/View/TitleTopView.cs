using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Reversi.UIs.View
{
    /// <summary>
    /// タイトルTOP
    /// </summary>
    public class TitleTopView : MonoBehaviour
    {
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        // MORE APP ボタン
        [SerializeField] private Button moreAppButton;
        public void SetListenerMoreAppButton(UnityAction action)
        {
            moreAppButton.onClick.RemoveAllListeners();
            moreAppButton.onClick.AddListener(action);
        }

        // ヘルプボタン
        [SerializeField] private Button helpButton;
        public void SetListenerHelpButton(UnityAction action)
        {
            helpButton.onClick.RemoveAllListeners();
            helpButton.onClick.AddListener(action);
        }

        // SEボタン
        [SerializeField] private Button seButton;
        [SerializeField] private Image seButtonImage;
        [SerializeField] private Sprite seOnSprite;
        [SerializeField] private Sprite seOffSprite;
        public void ChangeSeOnOffSprite(bool isOff)
        {
            seButtonImage.sprite = isOff ? seOffSprite : seOnSprite;
        }
        public void SetListenerSeButton(UnityAction action)
        {
            seButton.onClick.RemoveAllListeners();
            seButton.onClick.AddListener(action);
        }

        // BGMボタン
        [SerializeField] private Button bgmButton;
        [SerializeField] private Image bgmButtonImage;
        [SerializeField] private Sprite bgmOnSprite;
        [SerializeField] private Sprite bgmOffSprite;
        public void ChangeBgmOnOffSprite(bool isVolumeOff)
        {
            bgmButtonImage.sprite = isVolumeOff ? bgmOffSprite : bgmOnSprite;
        }
        public void SetListenerBgmButton(UnityAction action)
        {
            bgmButton.onClick.RemoveAllListeners();
            bgmButton.onClick.AddListener(action);
        }

        /// <summary>
        /// オセロ戦士バッジ
        /// </summary>
        [SerializeField] private GameObject reversiMasterBadge;
        public void SetActiveReversiMasterBadge(bool isActive)
        {
            reversiMasterBadge.SetActive(isActive);
        }
    }
}
