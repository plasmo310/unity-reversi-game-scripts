using UnityEngine;

namespace Reversi.Common
{
    public class DontDestroyBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
