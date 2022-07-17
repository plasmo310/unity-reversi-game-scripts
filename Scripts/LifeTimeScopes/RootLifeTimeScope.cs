using Reversi.Services;
using Reversi.Services.Impl;
using VContainer;
using VContainer.Unity;

namespace Reversi.LifeTimeScopes
{
    /// <summary>
    /// 全シーン共通 LifeTimeScope
    /// </summary>
    public class RootLifeTimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            // サービス登録
            builder.Register<IAssetsService, AssetsService>(Lifetime.Singleton);
            builder.Register<ILogService, LogDebugService>(Lifetime.Singleton);
        }
    }
}
