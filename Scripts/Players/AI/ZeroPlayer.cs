using System;
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
                // 通常：MiniMax -> モンテカルロ
                case PlayerEmotion.Normal:
                    var gameRate = StoneCalculator.GetGameRate(StoneStates);
                    SelectStoneIndex = gameRate < 0.3f
                        ? await SearchMiniMaxStoneTask()
                        : await SearchMonteCarloStoneTask();
                    break;
                // 焦り、悲しい：モンテカルロ
                case PlayerEmotion.Heat:
                case PlayerEmotion.Sad:
                    SelectStoneIndex = await SearchMonteCarloStoneTask();
                    break;
            }
        }

        /// <summary>
        /// MiniMax法でのストーン探索処理
        /// </summary>
        private async UniTask<StoneIndex> SearchMiniMaxStoneTask()
        {
            await UniTask.SwitchToThreadPool(); // 時間がかかるため別スレッドで実行
            var result = AIAlgorithm.SearchNegaAlphaStone(StoneStates, MyStoneState, 3, true);
            await UniTask.SwitchToMainThread();
            return result;
        }

        /// <summary>
        /// モンテカルロ探索処理
        /// </summary>
        private async UniTask<StoneIndex> SearchMonteCarloStoneTask()
        {
            await UniTask.SwitchToThreadPool(); // 時間がかかるため別スレッドで実行
            var gameRate = StoneCalculator.GetGameRate(StoneStates);
            var result = AIAlgorithm.SearchMonteCarloStone(StoneStates, MyStoneState, (int)　(150 * gameRate));
            await UniTask.SwitchToMainThread();
            return result;
        }
    }
}
