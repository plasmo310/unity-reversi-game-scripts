using Reversi.Managers;
using Reversi.Players.Input;
using Reversi.Players.Input.Impl;
using Reversi.Presenter;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Reversi
{
    /// <summary>
    /// Gameシーン LifeTimeScope
    /// </summary>
    public class GameLifeTimeScope : LifetimeScope
    {
        [SerializeField] private GamePresenter gamePresenter;

        protected override void Configure(IContainerBuilder builder)
        {
            // 各クラスを登録
            builder.Register<GameManager>(Lifetime.Singleton);
            builder.Register<BoardManager>(Lifetime.Singleton);
            builder.Register<StoneManager>(Lifetime.Singleton);
            builder.Register<PlayerManager>(Lifetime.Singleton);
            builder.Register<IInputEventProvider, InputEventProvider>(Lifetime.Singleton);

            // コンポーネント登録
            builder.RegisterComponent(gamePresenter);

            // エントリーポイント登録
            builder.RegisterEntryPoint<GameEntryPoint>();
        }
    }
}
