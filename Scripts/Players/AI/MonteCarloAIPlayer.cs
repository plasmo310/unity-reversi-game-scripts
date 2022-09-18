using System;
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
        /// <returns></returns>
        private async UniTask<StoneIndex> SearchStoneTask()
        {
            await UniTask.SwitchToThreadPool(); // 時間がかかるため別スレッドで実行
            var ratio = StoneCalculator.GetGameRate(StoneStates); // 終盤ほど数を増やすため進捗状況を取得
            var result = AIAlgorithm.SearchMonteCarloStone(StoneStates, MyStoneState, Mathf.RoundToInt(100 * ratio));
            await UniTask.SwitchToMainThread();
            return result;
        }
    }
}
