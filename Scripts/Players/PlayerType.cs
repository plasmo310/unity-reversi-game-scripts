namespace Reversi.Players
{
    /// <summary>
    /// プレイヤーの種類
    /// </summary>
    public enum PlayerType
    {
        InputPlayer,          // 入力プレイヤー
        RandomAIPlayer,       // ランダムに置くAI
        MiniMaxAIPlayer,      // MiniMaxアルゴリズムで置くAI
        MonteCarloAIPlayer,   // モンテカルロ法で置くAI
        MiniMaxMonteAIPlayer, // 序盤MiniMax法、終盤モンテカルロ法のAI
    }
}
