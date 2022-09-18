using Reversi.Cameras;
using Reversi.Data;
using Reversi.EntryPoints;
using Reversi.Managers;
using Reversi.Players.Display;
using Reversi.Settings;
using Reversi.UIs.Presenter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Reversi.LifeTimeScopes
{
    public class TitleLifeTimeScope : LifetimeScope
    {
        [SerializeField] private TitlePresenter titlePresenter;
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private PlayerTypeInfoData playerTypeInfoData;
        [SerializeField] private PlayerSelectBehaviour playerSelectBehaviour;
        [SerializeField] private TitleTopCamerasBehaviour titleTopCamerasBehaviour;
        [SerializeField] private SelectPlayerCamerasBehaviour selectPlayerCamerasBehaviour;

        protected override void Configure(IContainerBuilder builder)
        {
            // 各クラスを登録
            builder.Register<TitleManager>(Lifetime.Singleton);
            builder.Register<TitleCameraManager>(Lifetime.Singleton);

            // コンポーネント登録
            builder.RegisterComponent(titlePresenter);
            builder.RegisterComponent(playerSelectBehaviour);
            builder.RegisterComponent(titleTopCamerasBehaviour);
            builder.RegisterComponent(selectPlayerCamerasBehaviour);

            // インスタンス登録
            builder.RegisterInstance(gameSettings);
            builder.RegisterInstance(playerTypeInfoData);

            // エントリーポイント登録
            builder.RegisterEntryPoint<TitleEntryPoint>();
        }
    }
}
