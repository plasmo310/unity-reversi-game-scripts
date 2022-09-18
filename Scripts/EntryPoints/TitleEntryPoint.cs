using Reversi.Managers;
using VContainer;
using VContainer.Unity;

namespace Reversi.EntryPoints
{
    /// <summary>
    /// Titleシーン EntryPoint
    /// </summary>
    public class TitleEntryPoint : IStartable, ITickable
    {
        private readonly TitleManager _titleManager;

        [Inject]
        public TitleEntryPoint(TitleManager titleManager)
        {
            _titleManager = titleManager;
        }

        public void Start()
        {
            _titleManager.OnStart();
        }

        public void Tick()
        {
            _titleManager.OnUpdate();
        }
    }
}
