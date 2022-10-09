using Reversi.Common;
using Reversi.Data;
using Reversi.DB;
using Reversi.Services;
using Reversi.Services.Impl;
using Reversi.Settings;
using Reversi.UIs.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Reversi.LifeTimeScopes
{
    /// <summary>
    /// 全シーン共通 LifeTimeScope
    /// </summary>
    public class RootLifeTimeScope : LifetimeScope
    {
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private AdmobSettings admobSettings;
        [SerializeField] private PlayerTypeInfoData playerTypeInfoData;
        [SerializeField] private GameObject transitionViewPrefab;
        [SerializeField] private GameObject dialogViewPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            // モバイルプラットフォームか？
            // Editorでモバイルの挙動をテストしたい場合、ここをtrueにする
            if (Application.isMobilePlatform)
            {
                gameSettings.IsMobilePlatform = true;
            }

            // 遷移用View作成
            var transitionView = Instantiate(transitionViewPrefab).GetComponent<TransitionView>();
            transitionView.gameObject.AddComponent<DontDestroyBehaviour>();
            transitionView.SetActive(false); // 最初は非表示
            builder.RegisterComponent(transitionView);

            // ダイアログView作成
            var dialogView = Instantiate(dialogViewPrefab).GetComponent<DialogView>();
            dialogView.gameObject.AddComponent<DontDestroyBehaviour>();
            dialogView.SetActive(false);
            builder.RegisterComponent(dialogView);

            // リポジトリ登録
            // PlayerPrefsを使用しているサービスもあるため、最初に呼び出しておく
            builder.Register<PlayerPrefsRepository>(Lifetime.Singleton);
            builder.Register<IPlayerPrefsService, PlayerPrefsService>(Lifetime.Singleton);

            // サービス登録
            builder.Register<IAudioService, AudioService>(Lifetime.Singleton);
            builder.Register<IAssetsService, AssetsService>(Lifetime.Singleton);
            builder.Register<ILogService, LogDebugService>(Lifetime.Singleton);
            builder.Register<ITransitionService, TransitionService>(Lifetime.Singleton);
            builder.Register<IDialogService, DialogService>(Lifetime.Singleton);
            // モバイルの場合、Admobを適用する
            if (gameSettings.IsMobilePlatform)
            {
                builder.Register<IAdmobService, AdmobService>(Lifetime.Singleton);
            }
            else
            {
                builder.Register<IAdmobService, AdmobDummyService>(Lifetime.Singleton);
            }

            // インスタンス登録
            builder.RegisterInstance(gameSettings);
            builder.RegisterInstance(admobSettings);
            builder.RegisterInstance(playerTypeInfoData);
        }
    }
}
