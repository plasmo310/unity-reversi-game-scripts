using System;
using Cysharp.Threading.Tasks;
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
            StartThinkAsync();
        }

        /// <summary>
        /// 選択するストーンを考える
        /// </summary>
        private async void StartThinkAsync()
        {
            // 考える時間
            await UniTask.Delay(500);

            // ストーンを探索
            SelectStoneIndex = AIAlgorithm.SearchNegaAlphaStone(StoneStates, MyStoneState, 3, true);
        }
    }
}
