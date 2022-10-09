using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Reversi.Cameras
{
    /// <summary>
    /// プレイヤー全体カメラクラス
    /// </summary>
    public class AllPlayerCameraBehaviour : MonoBehaviour
    {
        /// <summary>
        /// CineMachine関連
        /// </summary>
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private CinemachineRecomposer cinemachineRecomposer;
        [SerializeField] private CinemachineCameraOffset cinemachineCameraOffset;

        /// <summary>
        /// ズームアニメーション
        /// </summary>
        private static readonly Vector3 InitZoomPosition = new (0.016f, 0.032f, 0.36f);  // ちょっと前方に置く
        private Tween _initZoomAnimation;

        /// <summary>
        /// パンアニメーション
        /// </summary>
        private static readonly float MaxPanValue = 17.0f;
        private Sequence _panAnimationSequence;
        private bool _isEnablePanAnimation;

        /// <summary>
        /// アニメーション中か？
        /// </summary>
        private bool _isDoAnimation = false;

        private void Awake()
        {
            _isEnablePanAnimation = false;
            cinemachineRecomposer.m_Pan = 0.0f;
            _isDoAnimation = false;

            // Zoomアニメーションの作成
            _initZoomAnimation = DOTween.To(
                () => cinemachineCameraOffset.m_Offset,
                (x) => cinemachineCameraOffset.m_Offset = x,
                Vector3.zero,
                0.8f
            ).SetEase(Ease.OutCubic).Pause().SetAutoKill(false).SetLink(gameObject);

            // 一時停止して自身と紐づける
            _initZoomAnimation
                .Pause()
                .SetAutoKill(false)
                .SetLink(gameObject);

            // Panアニメーションの作成
            _panAnimationSequence = DOTween.Sequence();
            _panAnimationSequence.Append(DOTween.To(
                () => cinemachineRecomposer.m_Pan,
                (x) => cinemachineRecomposer.m_Pan = x,
                MaxPanValue,
                3.0f).SetEase(Ease.Linear));
            _panAnimationSequence.Append(DOTween.To(
                () => cinemachineRecomposer.m_Pan,
                (x) => cinemachineRecomposer.m_Pan = x,
                0.0f,
                3.0f).SetEase(Ease.Linear));
            _panAnimationSequence.SetLoops(-1, LoopType.Restart);

            // 一時停止して自身と紐づける
            _panAnimationSequence
                .Pause()
                .SetLink(gameObject);

            OnResetAnimation();
        }

        private void Update()
        {
            // Priorityが下がった場合、アニメーションを停止する
            if (_isDoAnimation && cinemachineVirtualCamera.Priority < 0)
            {
                _isDoAnimation = false;
                OnResetAnimation();
            }
        }

        /// <summary>
        /// アニメーション再スタート
        /// VirtualCameraが有効になったタイミングで呼ばれる
        /// </summary>
        public async void OnAnimationRestart()
        {
            _isDoAnimation = true;

            _initZoomAnimation.Restart();

            if (!_isEnablePanAnimation) return;

            await UniTask.Delay(500);
            _panAnimationSequence.Restart();
        }

        /// <summary>
        /// アニメーションリセット
        /// </summary>
        private void OnResetAnimation()
        {
            // 一時停止して位置をリセットする
            _initZoomAnimation.Pause();
            cinemachineCameraOffset.m_Offset = InitZoomPosition;
            _panAnimationSequence.Pause();
            cinemachineRecomposer.m_Pan = 0.0f;
        }

        /// <summary>
        /// パンアニメーションを有効にする
        /// </summary>
        public void SetIsEnablePanAnimation(bool isEnable)
        {
            _isEnablePanAnimation = isEnable;
        }
    }
}
