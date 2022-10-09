using Reversi.Cameras;
using Reversi.Players;
using VContainer;

namespace Reversi.Managers
{
    /// <summary>
    /// Titleカメラ管理クラス
    /// </summary>
    public class TitleCameraManager
    {
        private readonly TitleTopCamerasBehaviour _titleTopCamerasBehaviour;
        private readonly SelectPlayerCamerasBehaviour _selectPlayerCamerasBehaviour;

        /// <summary>
        /// カメラを非表示にするPriority
        /// </summary>
        private static readonly int HideCameraPriority = -1;

        [Inject]
        public TitleCameraManager(TitleTopCamerasBehaviour titleTopCamerasBehaviour, SelectPlayerCamerasBehaviour selectPlayerCamerasBehaviour)
        {
            _titleTopCamerasBehaviour = titleTopCamerasBehaviour;
            _selectPlayerCamerasBehaviour = selectPlayerCamerasBehaviour;
        }

        private void SetAllVirtualCameraPriority(int priority)
        {
            _titleTopCamerasBehaviour.SetAllPriority(priority);
            _selectPlayerCamerasBehaviour.SetAllPriority(priority);
        }

        /// <summary>
        /// 状態に応じてカメラを変更する
        /// </summary>
        /// <param name="state"></param>
        public void ChangeCamera(TitleState state)
        {
            // 全てのカメラのPriorityを下げる
            SetAllVirtualCameraPriority(HideCameraPriority);

            // 状態に応じてカメラを切り替える
            switch (state)
            {
                case TitleState.SelectMode:
                    _titleTopCamerasBehaviour.StartAnimation();
                    break;
                case TitleState.SelectPlayer:
                    _titleTopCamerasBehaviour.StopAnimation();
                    _selectPlayerCamerasBehaviour.ShowCamera(PlayerType.None);
                    break;
            }
        }

        /// <summary>
        /// プレイヤー選択のカメラを切り替える
        /// </summary>
        /// <param name="playerType"></param>
        public void ChangeSelectPlayerCamera(PlayerType playerType)
        {
            _selectPlayerCamerasBehaviour.SetAllPriority(HideCameraPriority);
            _selectPlayerCamerasBehaviour.ShowCamera(playerType);
        }

        /// <summary>
        /// 全体カメラのパンアニメーションを有効にする
        /// </summary>
        public void SetIsEnableAllPlayerCameraPanAnimation(bool isEnable)
        {
            _selectPlayerCamerasBehaviour.SetIsEnableAllPlayerCameraPanAnimation(isEnable);
        }
    }
}
