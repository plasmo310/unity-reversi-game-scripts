using UnityEngine;
using UnityEngine.EventSystems;

namespace Reversi.Common
{
    /// <summary>
    /// Touch関連共通クラス
    /// </summary>
    public static class TouchUtil
    {
        // マウス左クリック
        private const string MouseLeftClick = "Fire1";
        // タッチしていない(id: -1)
        public static readonly Touch NotTouch = new Touch() { fingerId = -1 };

        /// <summary>
        /// 開始されたタッチオブジェクトを返却する
        /// </summary>
        public static Touch GetBeganTouch()
        {
            // *** Unityエディター ***
            if (Application.isEditor)
            {
                // クリックされた場合(ID:0固定)
                if (Input.GetButtonDown(MouseLeftClick))
                {
                    return new Touch()
                    {
                        fingerId = 0,
                        position = Input.mousePosition,
                    };
                }

                // クリックされていない場合
                return NotTouch;
            }

            // *** iOS・Android ***
            // 開始されたタッチオブジェクトを返す
            foreach (var t in Input.touches)
            {
                if (t.phase == TouchPhase.Began)
                    return t;
            }

            // タッチされていない場合
            return NotTouch;
        }

        /// <summary>
        /// タップがuGUIと重なっているか
        /// </summary>
        public static bool IsPointerOver(Touch touch)
        {
            // *** Unityエディター ***
            if (Application.isEditor)
            {
                return EventSystem.current &&
                       EventSystem.current.IsPointerOverGameObject();
            }

            // *** iOS・Android ***
            return EventSystem.current &&
                   EventSystem.current.IsPointerOverGameObject(touch.fingerId);
        }

        /// <summary>
        /// 画面がタッチされたかどうか？
        /// </summary>
        public static bool IsScreenTouch()
        {
            var touch = GetBeganTouch();
            if (touch.fingerId == -1) return false; // id=-1でない
            if (IsPointerOver(touch)) return false; // UIと重なっていない
            return true;
        }

        /// <summary>
        /// タッチIDに対する現在位置を取得
        /// </summary>
        public static Vector3 GetCurrentTouchPosition(Touch touch)
        {
            // *** Unityエディター ***
            if (Application.isEditor)
            {
                // マウスの位置を返却
                return Input.mousePosition;
            }

            // *** iOS・Android ***
            // タッチIDの位置を返却
            var currentPosition = new Vector3(0, 0, 0);
            foreach (var t in Input.touches)
            {
                if (t.fingerId == touch.fingerId)
                    currentPosition = t.position;
            }

            return currentPosition;
        }

        /// <summary>
        /// スクリーン座標からキャンバス座標に変換
        /// </summary>
        /// <param name="screenPos">スクリーン座標</param>
        /// <param name="canvasRectTransform">キャンバスTransform</param>
        /// <returns>キャンバス座標</returns>
        public static Vector3 ScreenToCanvas(Vector3 screenPos, RectTransform canvasRectTransform)
        {
            // スクリーン座標を割合に変換
            var ratioX = screenPos.x / Screen.width;
            var ratioY = screenPos.y / Screen.height;

            // 割合をキャンバス座標に当てはめる
            var canvasRect = canvasRectTransform.rect;
            var canvasX = ratioX * canvasRect.width;
            var canvasY = ratioY * canvasRect.height;

            // キャンバス座標は中心が0なので合わせる
            canvasX -= canvasRect.width * 0.5f;
            canvasY -= canvasRect.height * 0.5f;

            return new Vector3()
            {
                x = canvasX,
                y = canvasY,
                z = 0.0f,
            };
        }

        /// <summary>
        /// ドラッグ差分を算出する
        /// キャンバスTransformが指定されていればキャンバス座標に変換して返却
        /// </summary>
        /// <param name="tapPosition">タップ座標</param>
        /// <param name="dragPosition">ドラッグ座標</param>
        /// <param name="canvasRectTransform">キャンバスTransform</param>
        /// <returns>ドラッグ差分</returns>
        public static Vector3 CalculateDragDiffPosition(Vector3 tapPosition, Vector3 dragPosition,
            RectTransform canvasRectTransform = null)
        {
            // 指定されていればキャンバス座標に変換
            if (canvasRectTransform != null)
            {
                tapPosition = ScreenToCanvas(tapPosition, canvasRectTransform);
                dragPosition = ScreenToCanvas(dragPosition, canvasRectTransform);
            }
            return dragPosition - tapPosition;
        }
    }
}
