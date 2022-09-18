using System;
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
                // 通常、焦り：MiniMax
                case PlayerEmotion.Normal:
                case PlayerEmotion.Heat:
                    SelectStoneIndex = await SearchMiniMaxStoneTask();
                    break;
                // 悲しい：ランダム
                case PlayerEmotion.Sad:
                    SelectStoneIndex = AIAlgorithm.GetRandomStoneIndex(StoneStates, MyStoneState);
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
    }
}
