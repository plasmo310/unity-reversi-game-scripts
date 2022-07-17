using System;
using Reversi.Managers;
using Reversi.Players.Agents;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// MLAgentsを使用したAI
    /// </summary>
    public class MlAgentsAIPlayer : Player
    {
        private ReversiAIAgent _agent;
        public MlAgentsAIPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction) : base(myStoneState, putStoneAction) { }

        protected override void StartThink()
        {
            // エージェントクラスを取得
            if (_agent == null && PlayerGameObject != null)
            {
                _agent = PlayerGameObject.GetComponent<ReversiAIAgent>();
            }
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

            // ストーン探索処理
            var canPutStones = StoneCalculator.GetAllCanPutStonesIndex(StoneStates, MyStoneState);
            SelectStoneIndex = await _agent.OnSearchSelectStone(StoneStates, canPutStones.ToArray(), MyStoneState);
        }

        protected override void EndGame(PlayerResultState resultState)
        {
            // ゲームを終了させる
            _agent.OnGameEnd(resultState);
        }
    }
}
