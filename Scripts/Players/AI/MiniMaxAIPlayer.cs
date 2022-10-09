using System;
using System.Threading;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// MiniMaxアルゴリズムで判断するAI
    /// </summary>
    public class MiniMaxAIPlayer : Player
    {
        public MiniMaxAIPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction) { }

        protected override void StartThink()
        {
            StartThinkAsync(CancellationTokenSource.Token);
        }

        /// <summary>
        /// 選択するストーンを考える
        /// </summary>
        private async void StartThinkAsync(CancellationToken token)
        {
            // 考える時間
            await WaitSelectTime(200, token);

            // ストーンを探索
            SelectStoneIndex = await AIAlgorithm.SearchMultiThreadNegaAlphaStoneAsync(StoneStates, MyStoneState, 3, true, token);
        }
    }
}
