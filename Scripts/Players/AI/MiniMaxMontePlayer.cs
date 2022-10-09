using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// 序盤MiniMax法、終盤モンテカルロ法のAI
    /// </summary>
    public class MiniMaxMontePlayer : Player
    {
        public MiniMaxMontePlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction) { }

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
            SelectStoneIndex = await SearchStoneTask(token);
        }

        /// <summary>
        /// ストーン探索処理
        /// </summary>
        private async UniTask<StoneIndex> SearchStoneTask(CancellationToken token)
        {
            // 序盤はMiniMax、終盤はモンテカルロ法で探索する
            var gameRate = StoneCalculator.GetGameRate(StoneStates);
            var result = gameRate <= 0.8f
                ? await AIAlgorithm.SearchMultiThreadNegaAlphaStoneAsync(StoneStates, MyStoneState, 3, true, token)
                : await AIAlgorithm.SearchMultiThreadMonteCarloStoneAsync(StoneStates, MyStoneState, 100, token);
            return result;
        }
    }
}
