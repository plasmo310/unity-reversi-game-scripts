using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// ゴロヤン
    /// </summary>
    public class GoloyanPlayer : Player
    {
        public GoloyanPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction)
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

            // 奇抜さを出すため、1/5の確率でランダムに打つ
            var isRandom = UnityEngine.Random.Range(0, 5) == 0;
            if (isRandom)
            {
                SelectStoneIndex = AIAlgorithm.GetRandomStoneIndex(StoneStates, MyStoneState);
                return;
            }

            // 感情によって選択手法を変える
            var gameRate = StoneCalculator.GetGameRate(StoneStates);
            switch (Emotion)
            {
                // 通常：MiniMax -> モンテカルロ （稀にランダム）
                case PlayerEmotion.Normal:
                    SelectStoneIndex = gameRate < 0.5f
                        ? await AIAlgorithm.SearchMultiThreadNegaAlphaStoneAsync(StoneStates, MyStoneState, 3, true, token)
                        : await AIAlgorithm.SearchMultiThreadMonteCarloStoneAsync(StoneStates, MyStoneState, (int)　(80 * gameRate), token);
                    break;
                // 焦り：強化学習（MiniMonteキラー）
                case PlayerEmotion.Heat:
                    var canPutStones = StoneCalculator.GetAllCanPutStonesIndex(StoneStates, MyStoneState);
                    SelectStoneIndex = await PlayerAIAgent.OnSearchSelectStone(StoneStates, canPutStones.ToArray(), MyStoneState);
                    break;
                // 悲しい：モンテカルロ
                case PlayerEmotion.Sad:
                    SelectStoneIndex = await AIAlgorithm.SearchMultiThreadMonteCarloStoneAsync(StoneStates, MyStoneState, (int)　(80 * gameRate), token);
                    break;
            }
        }
    }
}
