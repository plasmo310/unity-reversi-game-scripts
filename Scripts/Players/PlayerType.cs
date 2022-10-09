using System.Collections.Generic;

namespace Reversi.Players
{
    /// <summary>
    /// プレイヤーの種類
    /// 追加したらPlayerFactoryも修正する
    /// </summary>
    public enum PlayerType
    {
        None,
        InputPlayer,             // 入力プレイヤー
        // ----- 本番で使用しないAI -----
        RandomAIPlayer,          // ランダムに置くAI
        MiniMaxAIPlayer,         // MiniMaxアルゴリズムで置くAI
        MonteCarloAIPlayer,      // モンテカルロ法で置くAI
        MiniMaxMonteAIPlayer,    // 序盤MiniMax法、終盤モンテカルロ法のAI
        MlAgentAIPlayer,         // MLAgentsを使用したAI
        MiniMonteKillerAIPlayer, // MiniMonteAI対策に特化したAI
        MlAgentAIPlayerLearn1,   // MLAgentsを使用したAI(学習用1)
        MlAgentAIPlayerLearn2,   // MLAgentsを使用したAI(学習用2)
        // ----- 本番用、進捗に影響しないAI -----
        MiniMaxAIRobotPlayer,    // AIロボ(MiniMax)
        MonteCarloAIRobotPlayer, // AIロボ(モンテカルロ)
        MlAgentAIRobotPlayer,    // AIロボ(強化学習)
        // ----- 本番用、進捗に影響するAI -----
        // 定義した順番で進捗が管理されるため入れ替えないこと！
        PikaruPlayer,    // ピカル
        MichaelPlayer,   // マイケル
        ElekiBearPlayer, // エレキベア
        GoloyanPlayer,   // ゴロヤン
        ZeroPlayer,      // ゼロ
    }

    public static class PlayerTypeUtil
    {
        /// <summary>
        /// AIロボットタイプ
        /// 同一のモデルを使用している
        /// </summary>
        private static readonly List<PlayerType> AIRobotType = new()
        {
            PlayerType.MiniMaxAIRobotPlayer,
            PlayerType.MonteCarloAIRobotPlayer,
            PlayerType.MlAgentAIRobotPlayer
        };
        public static bool IsAiRobotType(PlayerType playerType)
        {
            return AIRobotType.Contains(playerType);
        }
    }
}
