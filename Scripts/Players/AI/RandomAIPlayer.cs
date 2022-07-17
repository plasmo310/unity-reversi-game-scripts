using System;
using Cysharp.Threading.Tasks;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// ランダムにストーンを置くAI
    /// </summary>
    public class RandomAIPlayer : Player
    {
        public RandomAIPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction) { }

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

            // 早すぎると上手くいかないので1フレームは待つ
            await UniTask.DelayFrame(1);

            // ランダムに取得して設定
            var canPutStones = StoneCalculator.GetAllCanPutStonesIndex(StoneStates, MyStoneState);
            var randomIndex = UnityEngine.Random.Range(0, canPutStones.Count);
            SelectStoneIndex = canPutStones[randomIndex];
        }
    }
}
