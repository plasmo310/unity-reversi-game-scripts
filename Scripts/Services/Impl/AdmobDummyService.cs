using UnityEngine.Events;

namespace Reversi.Services.Impl
{
    public class AdmobDummyService : IAdmobService
    {
        public void ShowBanner()
        {
            // 処理なし
        }

        public void HideBanner()
        {
            // 処理なし
        }

        public void ShowInterstitial(UnityAction callback)
        {
            // コールバック実行
            callback?.Invoke();
        }

        public void DestroyInterstitialAd()
        {
            // 処理なし
        }
    }
}
