using System.Collections.Generic;
using AudioInfo = Reversi.Services.IAudioService.AudioInfo;

namespace Reversi.Audio
{
    /// <summary>
    /// オーディオタイプ
    /// </summary>
    public enum ReversiAudioType
    {
        None,
        BgmTitleTop, // タイトルTOP BGM
        BgmBattle,   // バトル BGM
        BgmBattleWin,
        BgmBattleLose,
        SeClick,  // クリック
        SeDecide, // 決定
        SePut1,
    }

    public static class ReversiAudioUtil
    {
        /// <summary>
        /// オーディオ情報を返却
        /// </summary>
        public static Dictionary<ReversiAudioType, AudioInfo> GetAudioInfos()
        {
            var audioInfos = new Dictionary<ReversiAudioType, AudioInfo>();
            audioInfos.Add(ReversiAudioType.BgmTitleTop, new AudioInfo("OthelloBeat", 0.8f));
            audioInfos.Add(ReversiAudioType.BgmBattle, new AudioInfo("OthelloBattle", 0.7f));
            audioInfos.Add(ReversiAudioType.BgmBattleWin, new AudioInfo("OthelloBeat", 0.8f));
            audioInfos.Add(ReversiAudioType.BgmBattleLose, new AudioInfo("OthelloBattleLose", 0.7f));
            audioInfos.Add(ReversiAudioType.SeClick, new AudioInfo("se_put2", 1.0f));
            audioInfos.Add(ReversiAudioType.SeDecide, new AudioInfo("se_decide", 0.6f));
            audioInfos.Add(ReversiAudioType.SePut1, new AudioInfo("se_put1", 1.0f));
            return audioInfos;
        }
    }
}
