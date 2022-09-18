using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Reversi.Cameras
{
    /// <summary>
    /// TitleTopカメラクラス
    /// </summary>
    public class TitleTopCamerasBehaviour : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCameraRound;
        [SerializeField] private CinemachineDollyCart dollyCartRound;
        [SerializeField] private CinemachineVirtualCamera virtualCameraDolly;
        [SerializeField] private CinemachineDollyCart dollyCartDolly;
        [SerializeField] private CinemachineVirtualCamera virtualCameraUp;
        [SerializeField] private CinemachineDollyCart dollyCartUp;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        private Vector3 _initVirtualCameraRoundPosition;
        private Quaternion _initVirtualCameraRoundRotation;

        /// <summary>
        /// アニメーション中か？
        /// </summary>
        private bool _isPlayAnimation;

        private void Awake()
        {
            _isPlayAnimation = false;
            _initVirtualCameraRoundPosition = virtualCameraRound.transform.localPosition;
            _initVirtualCameraRoundRotation = virtualCameraRound.transform.localRotation;
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        /// <summary>
        /// アニメーション開始
        /// </summary>
        public void StartAnimation()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            StartAnimationAsync(_cancellationTokenSource.Token);
        }

        /// <summary>
        /// アニメーション開始処理
        /// </summary>
        /// <param name="token"></param>
        private async void StartAnimationAsync(CancellationToken token)
        {
            _isPlayAnimation = true;
            virtualCameraRound.Priority = 1;

            // 順番にカメラを切り替える
            while (_isPlayAnimation)
            {
                // Round
                dollyCartRound.m_Position = 0.0f;
                await UniTask.Delay(500, cancellationToken: token);
                SetAllPriority(-1);
                virtualCameraRound.Priority = 1;
                dollyCartRound.m_Speed = 0.3f;
                await UniTask.Delay(3000, cancellationToken: token);
                // Dolly
                dollyCartDolly.m_Position = 0.0f;
                await UniTask.Delay(500, cancellationToken: token);
                SetAllPriority(-1);
                virtualCameraDolly.Priority = 1;
                await UniTask.Delay(3000, cancellationToken: token);
                // Up
                dollyCartUp.m_Position = 0.0f;
                await UniTask.Delay(500, cancellationToken: token);
                SetAllPriority(-1);
                virtualCameraUp.Priority = 1;
                await UniTask.Delay(3000, cancellationToken: token);
            }
        }

        /// <summary>
        /// アニメーション停止
        /// </summary>
        public void StopAnimation()
        {
            if (!_isPlayAnimation) return;
            _isPlayAnimation = false;
            _cancellationTokenSource?.Cancel();

            // 位置を戻しておく
            dollyCartRound.m_Position = 0.0f;
            virtualCameraRound.transform.localPosition = _initVirtualCameraRoundPosition;
            virtualCameraRound.transform.localRotation = _initVirtualCameraRoundRotation;
        }

        public void SetAllPriority(int priority)
        {
            virtualCameraRound.Priority = priority;
            virtualCameraDolly.Priority = priority;
            virtualCameraUp.Priority = priority;
        }
    }
}
