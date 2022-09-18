using Reversi.Common;
using Reversi.Stones.Stone;
using UnityEngine;

namespace Reversi.Players.Input.Impl
{
    public class InputEventProvider : IInputEventProvider
    {
        /// <summary>
        /// マウスクリックしたストーンを返却する
        /// </summary>
        /// <returns></returns>
        public StoneBehaviour GetSelectStone()
        {
            if (TouchUtil.IsScreenTouch())
            {
                var touch = TouchUtil.GetBeganTouch();
                var touchPosition = TouchUtil.GetCurrentTouchPosition(touch);
                var ray = Camera.main.ScreenPointToRay(touchPosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    if (hit.collider.gameObject.CompareTag(StoneBehaviour.TagName))
                    {
                        return hit.collider.gameObject.GetComponent<StoneBehaviour>();
                    }
                }
            }
            return null;
        }
    }
}
