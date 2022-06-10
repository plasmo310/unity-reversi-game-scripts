namespace Reversi.Players
{
    /// <summary>
    /// プレイヤーの種類
    /// </summary>
    public enum PlayerKind
    {
        InputPlayer,     // 入力プレイヤー
        RandomAIPlayer,  // ランダムに置くAI
        MiniMaxAIPlayer, // MiniMaxアルゴリズムで置くAI
    }
}
