namespace Reversi.Services
{
    public interface IPlayerPrefsService
    {
        public void SetInt(string key, int value);
        public int GetInt(string key, int defaultValue = 0);
        public void SetFloat(string key, float value);
        public float GetFloat(string key, float defaultValue = 0.0f);
        public void SetString(string key, string value);
        public string GetString(string key, string defaultValue = null);
        public void SetBool(string key, bool value);
        public bool GetBool(string key, bool defaultValue = false);
    }
}
