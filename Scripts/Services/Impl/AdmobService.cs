using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using Reversi.Settings;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Reversi.Services.Impl
{
    /// <summary>
    /// Admob関連サービス
    /// </summary>
    public class AdmobService : IAdmobService
    {
        /// <summary>
        /// 初期化済か？
        /// </summary>
        private bool _isInitialized = false;

        /// <summary>
        /// メインスレッド実行用Context
        /// </summary>
        private readonly SynchronizationContext _context;

        public AdmobService(AdmobSettings admobSettings)
        {
            // Contextを保持しておく
            _context = SynchronizationContext.Current;

            // シーン切り替え検知のためアンロード検知処理を追加
            SceneManager.sceneUnloaded += UnLoadScene;

            // 広告IDの設定
#if DEBUG && UNITY_ANDROID
            _bannerUnitId = admobSettings.debugAndroidBannerUnitId;
            _interstitialUnitId = admobSettings.debugAndroidInterstitialUnitId;
#elif DEBUG && UNITY_IOS
            _bannerUnitId = admobSettings.debugIosBannerUnitId;
            _interstitialUnitId = admobSettings.debugIosInterstitialUnitId;
#elif UNITY_ANDROID
            _bannerUnitId = admobSettings.releaseAndroidBannerUnitId;
            _interstitialUnitId = admobSettings.releaseAndroidInterstitialUnitId;
#elif UNITY_IOS
            _bannerUnitId = admobSettings.releaseIosBannerUnitId;
            _interstitialUnitId = admobSettings.releaseIosInterstitialUnitId;
#endif

#if UNITY_IOS
            // Admobの初期化前にトラッキング許可を設定する
            RequestConfiguration requestConfiguration =
                new RequestConfiguration.Builder()
                    .SetSameAppKeyEnabled(true).build();
            MobileAds.SetRequestConfiguration(requestConfiguration);
#endif

            // 初期化処理
            MobileAds.Initialize(initStatus =>
            {
                // 初期化済
                _isInitialized = true;
            });
        }

        /// <summary>
        /// Sceneアンロード検知
        /// </summary>
        /// <param name="scene"></param>
        void UnLoadScene(Scene scene)
        {
            // シーン切り替えでバナーが非表示になるためfalseに変更
            _isBannerShowing = false;
        }

        #region バナー広告
        /// <summary>
        /// バナー広告ID
        /// </summary>
        private readonly string _bannerUnitId;

        /// <summary>
        /// バナー広告
        /// </summary>
        private BannerView _bannerView;

        /// <summary>
        /// バナー広告表示中か？
        /// </summary>
        private bool _isBannerShowing = false;

        /// <summary>
        /// バナー広告表示
        /// </summary>
        public async void ShowBanner()
        {
            // 表示中の場合は処理を行わない
            if (_isBannerShowing) return;
            _isBannerShowing = true;

            // 初期化完了まで待つ
            await UniTask.WaitUntil(() => _isInitialized || !_isBannerShowing);

            // 表示するまでの間にHideが呼ばれていた場合は何もしない
            if (!_isBannerShowing)
            {
                return;
            }

            // バナー広告の初期化
            if (_bannerView == null)
            {
                _bannerView = new BannerView(_bannerUnitId, AdSize.Banner, AdPosition.Bottom);
                _bannerView.OnAdLoaded += BannerHandleOnAdLoaded;
                _bannerView.OnAdFailedToLoad += BannerHandleOnAdFailedToLoad;
                _bannerView.OnAdOpening += BannerHandleOnAdOpened;
                _bannerView.OnAdClosed += BannerHandleOnAdClosed;

                // 広告をリクエストする
                var request = new AdRequest.Builder().Build();
                _bannerView.LoadAd(request);
                return;
            }

            // バナー広告の表示
            _bannerView?.Show();
        }

        /// <summary>
        /// バナー広告非表示
        /// </summary>
        public void HideBanner()
        {
            // 表示中でない場合は処理を行わない
            if (!_isBannerShowing) return;
            _isBannerShowing = false;
            _bannerView?.Hide();
        }

        private void BannerHandleOnAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("HandleAdLoaded event received");
        }

        private void BannerHandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("HandleFailedToReceiveAd event received with message: "
                      + args.LoadAdError);
        }

        private void BannerHandleOnAdOpened(object sender, EventArgs args)
        {
            Debug.Log("HandleAdOpened event received");
        }

        private void BannerHandleOnAdClosed(object sender, EventArgs args)
        {
            Debug.Log("HandleAdClosed event received");
        }
        #endregion

        #region インタースティシャル広告

        /// <summary>
        /// インタースティシャル広告ID
        /// </summary>
        private readonly string _interstitialUnitId;

        /// <summary>
        /// インタースティシャル広告
        /// </summary>
        private InterstitialAd _interstitialAd;

        /// <summary>
        /// インタースティシャル広告コールバック
        /// </summary>
        private UnityAction _interstitialCallback;

        /// <summary>
        /// インタースティシャル広告表示
        /// </summary>
        public void ShowInterstitial(UnityAction callback)
        {
            _interstitialCallback = callback;

            // 広告初期化
            _interstitialAd = new InterstitialAd(_interstitialUnitId);
            _interstitialAd.OnAdLoaded += HandleOnAdLoaded;
            _interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
            _interstitialAd.OnAdOpening += HandleOnAdOpening;
            _interstitialAd.OnAdClosed += HandleOnAdClosed;

            // 広告リクエスト
            var request = new AdRequest.Builder().Build();
            _interstitialAd.LoadAd(request);

            // 広告再生
            ShowInterstitialAsync();
        }

        private async void ShowInterstitialAsync()
        {
            // Loadを待ってから再生
            await UniTask.WaitUntil(_interstitialAd.IsLoaded);
            _interstitialAd.Show();
        }

        /// <summary>
        /// インタースティシャル広告破棄
        /// </summary>
        public void DestroyInterstitialAd()
        {
            _interstitialAd.Destroy();
        }

        // 広告の読み込み完了時
        private void HandleOnAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("HandleAdLoaded event received");
        }

        // 広告の読み込み失敗時
        private void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("HandleFailedToReceiveAd event received with message");

            // コールバック実行
            // ハンドラは別スレッドのためメインスレッドで実行
            _context.Post(_ =>
            {
                _interstitialCallback?.Invoke();
                _interstitialCallback = null;
            }, null);
        }

        // 広告がデバイスの画面いっぱいに表示されたとき
        private void HandleOnAdOpening(object sender, EventArgs args)
        {
            Debug.Log("HandleAdOpening event received");
        }

        // 広告を閉じたとき
        private void HandleOnAdClosed(object sender, EventArgs args)
        {
            Debug.Log("HandleAdClosed event received");


            // コールバック実行
            // ハンドラは別スレッドのためメインスレッドで実行
            _context.Post(_ =>
            {
                _interstitialCallback?.Invoke();
                _interstitialCallback = null;
            }, null);
        }
        #endregion
    }
}
