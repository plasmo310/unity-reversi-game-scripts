using System;
using System.Collections.Generic;
using UnityEngine;

namespace Reversi.Players.Display
{
    /// <summary>
    /// バトル中のプレイヤーカメラのオフセット情報
    /// </summary>
    public class PlayerCameraOffsetInfos : MonoBehaviour
    {
        /// <summary>
        /// デフォルトの設定
        /// </summary>
        [SerializeField] private CameraOffsetInfo defaultCameraOffsetInfoInfo;

        /// <summary>
        /// 各プレイヤーごとの設定
        /// </summary>
        [SerializeField] private List<CameraOffsetInfo> cameraOffsetInfos;
        [Serializable] public class CameraOffsetInfo
        {
            public PlayerType playerType;
            public GameObject cameraOffset;
        }

        /// <summary>
        /// 指定プレイヤーのオフセット情報を返却
        /// </summary>
        /// <param name="playerType">プレイヤー種類</param>
        /// <param name="isReverseBoard">カメラ位置をボードに対して反転させる（プレイヤー2の場合にtrueを設定する）</param>
        public CameraOffsetInfo GetCameraOffsetInfo(PlayerType playerType, bool isReverseBoard)
        {
            var cameraOffsetInfo = cameraOffsetInfos.Find(x => x.playerType == playerType)
                               ?? defaultCameraOffsetInfoInfo; // 設定されていない場合、デフォルトの設定を返却
            if (isReverseBoard)
            {
                // position xの値を反転
                var position = cameraOffsetInfo.cameraOffset.transform.position;
                position.x *= -1.0f;
                cameraOffsetInfo.cameraOffset.transform.position = position;
                // eulerAngles yを-60
                var rotation = cameraOffsetInfo.cameraOffset.transform.eulerAngles;
                rotation.y -= 60.0f;
                cameraOffsetInfo.cameraOffset.transform.eulerAngles = rotation;
            }
            return cameraOffsetInfo;
        }
    }
}
