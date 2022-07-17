namespace Reversi.Players
{
    /// <summary>
    /// プレイヤーの種類
    /// 追加したらPlayerFactoryも修正する
    /// </summary>
    public enum PlayerType
    {
        InputPlayer,           // 入力プレイヤー
        RandomAIPlayer,        // ランダムに置くAI
        MiniMaxAIPlayer,       // MiniMaxアルゴリズムで置くAI
        MonteCarloAIPlayer,    // モンテカルロ法で置くAI
        MiniMaxMonteAIPlayer,  // 序盤MiniMax法、終盤モンテカルロ法のAI
        MlAgentAIPlayer,       // MLAgentsを使用したAI
        MlAgentAIPlayerLearn1, // MLAgentsを使用したAI(学習用1)
        MlAgentAIPlayerLearn2, // MLAgentsを使用したAI(学習用2)
    }
}
