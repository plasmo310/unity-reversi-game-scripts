using System;
using System.Threading;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// MLAgentsを使用したAI
    /// </summary>
    public class MlAgentsAIPlayer : Player
    {
        public MlAgentsAIPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction) { }

        protected override void StartThink()
        {
            // 思考開始
            StartThinkAsync(CancellationTokenSource.Token);
        }

        /// <summary>
        /// 選択するストーンを考える
        /// </summary>
        private async void StartThinkAsync(CancellationToken token)
        {
            // 考える時間
            await WaitSelectTime(200, token);

            // ストーン探索処理
            var canPutStones = StoneCalculator.GetAllCanPutStonesIndex(StoneStates, MyStoneState);
            SelectStoneIndex = await PlayerAIAgent.OnSearchSelectStone(StoneStates, canPutStones.ToArray(), MyStoneState);
        }
    }
}
