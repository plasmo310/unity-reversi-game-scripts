using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// エレキベア
    /// </summary>
    public class ElekiBearPlayer : Player
    {
        public ElekiBearPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction)
        {
        }

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

            // 早すぎると上手くいかないので1フレームは待つ
            await UniTask.DelayFrame(1, cancellationToken: token);

            // 感情によって選択手法を変える
            switch (Emotion)
            {
                // 通常、焦り：MiniMax
                case PlayerEmotion.Normal:
                case PlayerEmotion.Heat:
                    SelectStoneIndex = await AIAlgorithm.SearchMultiThreadNegaAlphaStoneAsync(StoneStates, MyStoneState, 3, true, token);
                    break;
                // 悲しい：ランダム
                case PlayerEmotion.Sad:
                    SelectStoneIndex = AIAlgorithm.GetRandomStoneIndex(StoneStates, MyStoneState);
                    break;
            }
        }
    }
}
