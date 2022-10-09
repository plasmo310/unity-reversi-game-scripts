using System;
using System.Collections.Generic;
using Reversi.Players;
using Reversi.Services;
using VContainer;

namespace Reversi.DB
{
    /// <summary>
    /// PlayerPrefsリポジトリ
    /// </summary>
    public class PlayerPrefsRepository
    {
        private readonly IPlayerPrefsService _playerPrefsService;

        [Inject]
        public PlayerPrefsRepository(IPlayerPrefsService playerPrefsService)
        {
            _playerPrefsService = playerPrefsService;

            // 各AIの解放メッセージを設定
            _releasePlayerMessageDictionary = new Dictionary<PlayerType, List<string>>();
            _releasePlayerMessageDictionary.Add(PlayerType.PikaruPlayer, new List<string> { "勝利おめでとう!!<br>教室にクラスメイトの<br>マイケル もやってきたようです!" });
            _releasePlayerMessageDictionary.Add(PlayerType.MichaelPlayer, new List<string> { "勝利おめでとう!!<br>教室にクラスメイトの<br>エレキベア もやってきたようです!",
                "観戦モードが解放されました!<br>プレイヤー同士を対戦させて遊んでみよう!" }); // 3種類以上のキャラ撃破で解放する条件になっている
            _releasePlayerMessageDictionary.Add(PlayerType.ElekiBearPlayer, new List<string> { "勝利おめでとう!!<br>教室に 野生のゴロヤン<br>もやってきたようです!" });
            _releasePlayerMessageDictionary.Add(PlayerType.GoloyanPlayer, new List<string> { "噂を聞きつけて、クラスで一番強いオセロ戦士がやってきたようです...!" });
            _releasePlayerMessageDictionary.Add(PlayerType.ZeroPlayer, new List<string> {
                "ここまで遊んでくれてありがとう!!<br>このクラスで一番強いあなたはオセロ戦士です!",
                "最後におまけでシミュレーションAIを解放しました!<br>アルゴリズムにどんな違いがあるのか楽しんでみてください!" });
        }

        // ---------- プレイヤー撃破情報関連 ----------
        /// <summary>
        /// 最後に倒した（最も進んだ）プレイヤー種別
        /// </summary>
        private static readonly string SaveKeyMaxDefeatPlayer = "SaveKeyMaxDefeatPlayer";

        /// <summary>
        /// 倒したプレイヤー種別に対して解放するプレイヤー情報
        /// ※このクラスで管理するものではないと思うが、面倒くさいので許容
        /// </summary>
        private readonly Dictionary<PlayerType, List<string>> _releasePlayerMessageDictionary = null;

        /// <summary>
        /// 最後に倒した（最も進んだ）プレイヤー種別 保存処理
        /// </summary>
        /// <param name="defeatPlayerType"></param>
        /// <returns>保存に成功したか？</returns>
        public bool SaveMaxDefeatPlayerType(PlayerType defeatPlayerType)
        {
            var isSave = false;

            // 既に倒したプレイヤー種別より進んでいれば保存する
            var defeatPlayer = (int) defeatPlayerType;
            var saveLastDefeatPlayer = _playerPrefsService.GetInt(SaveKeyMaxDefeatPlayer);
            if (defeatPlayer > saveLastDefeatPlayer)
            {
                _playerPrefsService.SetInt(SaveKeyMaxDefeatPlayer, defeatPlayer);
                isSave = true;
            }
            return isSave;
        }
        public PlayerType GetMaxDefeatPlayerType()
        {
            var saveMaxDefeatPlayer = _playerPrefsService.GetInt(SaveKeyMaxDefeatPlayer);
            return (PlayerType) Enum.ToObject(typeof(PlayerType), saveMaxDefeatPlayer);
        }

        /// <summary>
        /// AI解放時のメッセージを返却
        /// </summary>
        public List<string> GetReleasePlayerMessages(PlayerType defeatPlayerType)
        {
            return _releasePlayerMessageDictionary[defeatPlayerType];
        }

        /// <summary>
        /// 解放済プレイヤーの取得
        /// </summary>
        public List<PlayerType> GetReleasePlayers()
        {
            var allReleasePlayerList = new List<PlayerType>();

            // ピカルはデフォルトで解放済
            var defaultPlayerType = PlayerType.PikaruPlayer;
            allReleasePlayerList.Add(defaultPlayerType);

            // 解放するAIの判定
            var maxDefeatPlayer = _playerPrefsService.GetInt(SaveKeyMaxDefeatPlayer);
            var releasePlayer = maxDefeatPlayer + 1; // 倒した敵の次の敵を解放
            var defaultPlayer = (int) defaultPlayerType;
            foreach (PlayerType playerType in Enum.GetValues(typeof(PlayerType)))
            {
                // デフォルトAI〜解放対象までのAIをターゲットに加える
                var player = (int) playerType;
                if (releasePlayer >= player && player > defaultPlayer)
                {
                    allReleasePlayerList.Add(playerType);
                }
            }

            // 最後の敵まで倒している場合、AIロボットを追加する
            var bossPlayer = Enum.GetNames(typeof(PlayerType)).Length - 1;
            if (maxDefeatPlayer >= bossPlayer)
            {
                allReleasePlayerList.Add(PlayerType.MiniMaxAIRobotPlayer);
                allReleasePlayerList.Add(PlayerType.MonteCarloAIRobotPlayer);
                allReleasePlayerList.Add(PlayerType.MlAgentAIRobotPlayer);
            }
            return allReleasePlayerList;
        }

        /// <summary>
        /// ボスを撃破済か？
        /// </summary>
        /// <returns></returns>
        public bool IsDefeatLastBoss()
        {
            var maxDefeatPlayer = _playerPrefsService.GetInt(SaveKeyMaxDefeatPlayer);
            return maxDefeatPlayer >= (int) PlayerType.ZeroPlayer;
        }

        // ---------- オーディオ関連 ----------
        /// <summary>
        /// SEボリュームオフフラグ
        /// </summary>
        private static readonly string SaveKeyIsSeVolumeOff = "SaveKeyIsSeVolumeOff";
        public bool GetIsSeVolumeOff()
        {
            return _playerPrefsService.GetBool(SaveKeyIsSeVolumeOff);
        }
        public void SaveIsSeVolumeOff(bool isVolumeOff)
        {
            _playerPrefsService.SetBool(SaveKeyIsSeVolumeOff, isVolumeOff);
        }

        /// <summary>
        /// BGMボリュームオフフラグ
        /// </summary>
        private static readonly string SaveKeyIsBgmVolumeOff = "SaveKeyIsBgmVolumeOff";
        public bool GetIsBgmVolumeOff()
        {
            return _playerPrefsService.GetBool(SaveKeyIsBgmVolumeOff);
        }
        public void SaveIsBgmVolumeOff(bool isVolumeOff)
        {
            _playerPrefsService.SetBool(SaveKeyIsBgmVolumeOff, isVolumeOff);
        }
    }
}
