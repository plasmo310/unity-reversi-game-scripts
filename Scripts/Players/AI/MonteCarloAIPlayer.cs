using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Stones;
using Reversi.Stones.Stone;
using UnityEngine;

namespace Reversi.Players.AI
{
    /// <summary>
    /// モンテカルロ法で判断するAI
    /// </summary>
    public class MonteCarloAIPlayer : Player
    {
        public MonteCarloAIPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction) { }

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
            var ratio = StoneCalculator.GetGameRate(StoneStates); // 終盤ほど数を増やすため進捗状況を取得
            var result = await AIAlgorithm.SearchMultiThreadMonteCarloStoneAsync(StoneStates, MyStoneState, Mathf.RoundToInt(90 * ratio), token);
            return result;
        }
    }
}
