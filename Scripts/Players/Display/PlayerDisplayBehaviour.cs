using Reversi.Const;
using Reversi.Extensions;
using UnityEngine;

namespace Reversi.Players.Display
{
    /// <summary>
    /// プレイヤー表示用Behaviour
    /// </summary>
    public class PlayerDisplayBehaviour : MonoBehaviour
    {
        /// <summary>
        /// プレイヤーカメラ
        /// </summary>
        [SerializeField] private GameObject playerCamera;
        [SerializeField] private PlayerCameraOffsetInfos playerCameraOffsetInfos;

        public void Initialize(PlayerType playerType)
        {
            // 自身に設定されているレイヤーを子オブジェクト以下にも設定してRenderTextureに描画する
            var layer = gameObject.layer;
            gameObject.SetLayerRecursively(layer);

            // カメラ位置を調整
            var isReverseBoard = LayerMask.LayerToName(layer) == GameConst.LayerNamePlayer2;
            var cameraOffsetInfo = playerCameraOffsetInfos.GetCameraOffsetInfo(playerType, isReverseBoard);
            playerCamera.transform.localPosition = cameraOffsetInfo.cameraOffset.transform.localPosition;
            playerCamera.transform.localRotation = cameraOffsetInfo.cameraOffset.transform.localRotation;
        }
    }
}
