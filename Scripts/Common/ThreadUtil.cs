using UnityEngine;

namespace Reversi.Common
{
    public static class ThreadUtil
    {
        /// <summary>
        /// マルチスレッドに切り替えることができるか？
        /// </summary>
        public static bool IsSupportMultiThread()
        {
            // WebGLはシングルスレッドのため切り替えることができない
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                return false;
            }
            return true;
        }
    }
}
