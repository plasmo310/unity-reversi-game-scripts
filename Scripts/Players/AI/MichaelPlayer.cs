using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// マイケル
    /// </summary>
    public class MichaelPlayer : Player
    {
        public MichaelPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction)
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
                // 通常：強化学習（強）
                case PlayerEmotion.Normal:
                    var canPutStones = StoneCalculator.GetAllCanPutStonesIndex(StoneStates, MyStoneState);
                    SelectStoneIndex = await PlayerAIAgent.OnSearchSelectStone(StoneStates, canPutStones.ToArray(), MyStoneState);
                    break;
                // 焦り：ランダム
                case PlayerEmotion.Heat:
                    SelectStoneIndex = AIAlgorithm.GetRandomStoneIndex(StoneStates, MyStoneState);
                    break;
                // 悲しい：モンテカルロ
                case PlayerEmotion.Sad:
                    var gameRate = StoneCalculator.GetGameRate(StoneStates);
                    SelectStoneIndex = await AIAlgorithm.SearchMultiThreadMonteCarloStoneAsync(StoneStates, MyStoneState, (int)(100 * gameRate), token);
                    break;
            }
        }
    }
}
