using Reversi.Board;
using Reversi.EntryPoints;
using Reversi.Managers;
using Reversi.Players;
using Reversi.Players.Input;
using Reversi.Players.Input.Impl;
using Reversi.Services;
using Reversi.Services.Impl;
using Reversi.Settings;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Reversi.LifeTimeScopes
{
    /// <summary>
    /// 学習用シーン LifeTimeScope
    /// </summary>
    public class LearnGameLifeTimeScope : LifetimeScope
    {
        [SerializeField] private BoardBehaviour boardBehaviour;
        [SerializeField] private GameSettings gameSettings;

        protected override void Configure(IContainerBuilder builder)
        {
            // 各クラスを登録
            builder.Register<GameManager>(Lifetime.Singleton);
            builder.Register<StoneManager>(Lifetime.Singleton);
            builder.Register<PlayerManager>(Lifetime.Singleton);
            builder.Register<IInputEventProvider, InputEventProvider>(Lifetime.Singleton);
            builder.Register<PlayerFactory>(Lifetime.Singleton);

            // コンポーネント登録
            builder.RegisterComponent(boardBehaviour);

            // インスタンス登録
            builder.RegisterInstance(gameSettings);

            // エントリーポイント登録
            builder.RegisterEntryPoint<GameEntryPoint>();

            // デバッグ用設定
            if (gameSettings.DebugOption.isWriteDebugLog)
            {
                builder.Register<ILogService, LogWriterService>(Lifetime.Singleton);
            }
        }
    }
}
