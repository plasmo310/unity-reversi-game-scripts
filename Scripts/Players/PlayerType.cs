namespace Reversi.Players
{
    /// <summary>
    /// プレイヤーの種類
    /// </summary>
    public enum PlayerType
    {
        InputPlayer,     // 入力プレイヤー
        RandomAIPlayer,  // ランダムに置くAI
        MiniMaxAIPlayer, // MiniMaxアルゴリズムで置くAI
    }
}
