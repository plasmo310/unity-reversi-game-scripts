using System;
using System.Collections.Generic;
using Cinemachine;
using Reversi.Players;
using UnityEngine;

namespace Reversi.Cameras
{
    /// <summary>
    /// プレイヤー選択カメラクラス
    /// </summary>
    public class SelectPlayerCamerasBehaviour : MonoBehaviour
    {
        /// <summary>
        /// カメラ情報
        /// </summary>
        [SerializeField] private List<CameraInfo> cameraInfos;
        [Serializable] private class CameraInfo
        {
            /// <summary>
            /// プレイヤー種類
            /// </summary>
            public PlayerType playerType;
            /// <summary>
            /// プレイヤー種類に応じたカメラ
            /// </summary>
            public CinemachineVirtualCamera virtualCamera;
        }

        /// <summary>
        /// プレイヤー種類に応じたカメラを表示する
        /// </summary>
        /// <param name="playerType"></param>
        public void ShowCamera(PlayerType playerType)
        {
            // プレイヤー種類に応じたカメラを取得して表示
            var cameraInfo = cameraInfos.Find(info => info.playerType == playerType);
            if (cameraInfo == null)
            {
                Debug.LogError("noting set camera!! type: " + playerType);
                return;
            }
            cameraInfo.virtualCamera.Priority = 1;
        }

        public void SetAllPriority(int priority)
        {
            cameraInfos.ForEach(info => info.virtualCamera.Priority = priority);
        }

        /// <summary>
        /// 全体を映すカメラ
        /// </summary>
        [SerializeField] private AllPlayerCameraBehaviour allPlayerCameraBehaviour;

        /// <summary>
        /// 全体カメラのパンアニメーションを有効にする
        /// </summary>
        public void SetIsEnableAllPlayerCameraPanAnimation(bool isEnable)
        {
            allPlayerCameraBehaviour.SetIsEnablePanAnimation(isEnable);
        }
    }
}
