using UnityEngine;

namespace Reversi.Services.Impl
{
    public class PlayerPrefsService : IPlayerPrefsService
    {
        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(key, value);
            SaveData();
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(key, defaultValue);
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);
            SaveData();
        }

        public float GetFloat(string key, float defaultValue = 0.0f)
        {
            return PlayerPrefs.GetFloat(key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(key, value);
            SaveData();
        }

        public string GetString(string key, string defaultValue = null)
        {
            return PlayerPrefs.GetString(key, defaultValue);
        }

        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            SaveData();
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            var defaultValueInt = defaultValue ? 1 : 0;
            return PlayerPrefs.GetInt(key, defaultValueInt) == 1;
        }

        private void SaveData()
        {
            PlayerPrefs.Save();
        }
    }
}
