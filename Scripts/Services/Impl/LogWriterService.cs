using System;
using System.IO;
using UnityEngine;

namespace Reversi.Services.Impl
{
    public class LogWriterService : ILogService
    {
        private static string LogFilePath = "./Assets/Reversi/DebugLog/";
        private readonly string _logFileName;

        public LogWriterService()
        {
            // 日時をファイル名に指定
            _logFileName = "debug_log_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        }

        public void PrintLog(string text)
        {
            Debug.Log("*** write text: " + text);
            // テキストへ出力
            var sw = new StreamWriter(LogFilePath + _logFileName, true);
            sw.WriteLine(DateTime.Now.ToString("yyyyMMddHHmmss: ") + text);
            sw.Flush();
            sw.Close();
        }
    }
}
