using Reversi.Managers;

namespace Reversi.Services
{
    public interface IAudioService
    {
        /// <summary>
        /// 効果音再生
        /// </summary>
        public void PlayOneShot(ReversiAudioType audioType);

        /// <summary>
        /// BGM再生
        /// </summary>
        public void PlayBGM(ReversiAudioType audioType);

        /// <summary>
        /// BGM停止
        /// </summary>
        public void StopBGM();

        /// <summary>
        /// SEピッチ変更
        /// </summary>
        /// <param name="pitch">ピッチ値</param>
        public void ChangeSePitch(float pitch);

        /// <summary>
        /// BGMボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeBgmVolume(float volume);

        /// <summary>
        /// SEボリューム変更
        /// </summary>
        /// <param name="volume"></param>
        public void ChangeSeVolume(float volume);
    }
}
