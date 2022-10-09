using Reversi.Const;
using Reversi.UIs.View;
using UnityEngine.SceneManagement;

namespace Reversi.Services.Impl
{
    public class TransitionService : ITransitionService
    {
        private readonly TransitionView _transitionView;
        private bool _isDoTransitionAnimation;

        public TransitionService(TransitionView transitionView)
        {
            _transitionView = transitionView;
            _isDoTransitionAnimation = false;

            // シーンロード完了処理を追加
            SceneManager.sceneLoaded += LoadedScene;
        }

        /// <summary>
        /// シーンロード処理
        /// </summary>
        /// <param name="nextScene"></param>
        public void LoadScene(string nextScene)
        {
            // 遷移アニメーション中に呼ばれた場合、処理を行わずに遷移する
            if (_isDoTransitionAnimation)
            {
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
                return;
            }

            // アニメーション再生後に遷移する
            _isDoTransitionAnimation = true;
            _transitionView.StartTransition(() =>
            {
                SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
            });
        }

        /// <summary>
        /// シーンロード完了処理
        /// </summary>
        /// <param name="nextScene"></param>
        /// <param name="mode"></param>
        private void LoadedScene(Scene nextScene, LoadSceneMode mode)
        {
            // インタースティシャル広告のシーンでは行わない
            if (SceneManager.GetActiveScene().name == GameConst.SceneNameInterstitial)
            {
                return;
            }

            // 遷移アニメーション中であれば終了させる
            if (_isDoTransitionAnimation)
            {
                _isDoTransitionAnimation = false;
                _transitionView.EndTransition();
            }
        }
    }
}
