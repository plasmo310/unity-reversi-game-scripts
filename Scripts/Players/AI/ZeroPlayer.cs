using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// ゼロ（オセロ戦士）
    /// </summary>
    public class ZeroPlayer : Player
    {
        public ZeroPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction)
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
            var gameRate = StoneCalculator.GetGameRate(StoneStates);
            switch (Emotion)
            {
                // 通常：MiniMax -> モンテカルロ
                case PlayerEmotion.Normal:
                    SelectStoneIndex = gameRate < 0.3f
                        ? await AIAlgorithm.SearchMultiThreadNegaAlphaStoneAsync(StoneStates, MyStoneState, 3, true, token)
                        : await AIAlgorithm.SearchMultiThreadMonteCarloStoneAsync(StoneStates, MyStoneState, (int)　(100 * gameRate), token);
                    break;
                // 焦り、悲しい：モンテカルロ
                case PlayerEmotion.Heat:
                case PlayerEmotion.Sad:
                    SelectStoneIndex = await AIAlgorithm.SearchMultiThreadMonteCarloStoneAsync(StoneStates, MyStoneState, (int)　(100 * gameRate), token);
                    break;
            }
        }
    }
}
