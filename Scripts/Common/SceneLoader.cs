using UnityEngine.SceneManagement;

namespace Reversi.Common
{
    public static class SceneLoader
    {
        public static void LoadScene(string nextScene)
        {
            // TODO 画面遷移アニメーションをどうするか？等を検討する
            SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
        }
    }
}
