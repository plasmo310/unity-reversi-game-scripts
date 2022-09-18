using UnityEngine;

namespace Reversi
{
    public static class ProjectInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // 30FPSに指定
            Application.targetFrameRate = 30;
        }
    }
}
