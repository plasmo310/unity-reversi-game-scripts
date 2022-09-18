using UnityEngine;

namespace Reversi.Extensions
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 子オブジェクト含めてLayerを設定する
        /// </summary>
        public static void SetLayerRecursively(this GameObject self, int layer)
        {
            self.layer = layer;
            foreach (Transform t in self.transform)
            {
                SetLayerRecursively(t.gameObject, layer);
            }
        }
    }
}
