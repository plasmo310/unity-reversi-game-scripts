using System;
using Cysharp.Threading.Tasks;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// ピカル
    /// </summary>
    public class PikaruPlayer : Player
    {
        public PikaruPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction) { }

        protected override void StartThink()
        {
            // 思考開始
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

            // 感情によって選択手法を変える
            switch (Emotion)
            {
                // 通常、悲しい：強化学習（弱）
                case PlayerEmotion.Normal:
                case PlayerEmotion.Sad:
                    var canPutStones = StoneCalculator.GetAllCanPutStonesIndex(StoneStates, MyStoneState);
                    SelectStoneIndex = await PlayerAIAgent.OnSearchSelectStone(StoneStates, canPutStones.ToArray(), MyStoneState);
                    break;
                // 焦り：ランダム
                case PlayerEmotion.Heat:
                    SelectStoneIndex = AIAlgorithm.GetRandomStoneIndex(StoneStates, MyStoneState);
                    break;
            }
        }
    }
}
