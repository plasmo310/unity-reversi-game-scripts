using UnityEngine;

namespace Reversi.Services.Impl
{
    public class LogDebugService : ILogService
    {
        public void PrintLog(string text)
        {
            Debug.Log(text);
        }
    }
}
