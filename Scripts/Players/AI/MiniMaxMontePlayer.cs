using System;
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
            StartThinkAsync();
        }

        /// <summary>
        /// 選択するストーンを考える
        /// </summary>
        private async void StartThinkAsync()
        {
            // 考える時間
            await WaitSelectTime(200);

            // ストーンを探索
            SelectStoneIndex = await SearchStoneTask();
        }

        /// <summary>
        /// ストーン探索処理
        /// </summary>
        private async UniTask<StoneIndex> SearchStoneTask()
        {
            // 時間がかかるため別スレッドで実行
            await UniTask.SwitchToThreadPool();

            // 序盤はMiniMax、終盤はモンテカルロ法で探索する
            var gameRate = StoneCalculator.GetGameRate(StoneStates);
            var result = gameRate <= 0.8f
                ? AIAlgorithm.SearchNegaAlphaStone(StoneStates, MyStoneState, 3, true)
                : AIAlgorithm.SearchMonteCarloStone(StoneStates, MyStoneState, 100);

            await UniTask.SwitchToMainThread();
            return result;
        }
    }
}
