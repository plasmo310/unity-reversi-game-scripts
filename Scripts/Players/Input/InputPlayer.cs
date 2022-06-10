using System;
using Reversi.Stones.Stone;

namespace Reversi.Players.Input
{
    /// <summary>
    /// 入力プレイヤー
    /// </summary>
    public class InputPlayer : Player
    {
        private readonly IInputEventProvider _inputEventProvider;

        public InputPlayer(StoneState myStoneState, Action<StoneState, int, int> putStoneAction, IInputEventProvider inputEventProvider) : base(myStoneState, putStoneAction)
        {
            _inputEventProvider = inputEventProvider;
        }

        protected override void UpdateThink()
        {
            // 入力検知したストーンを取得
            var selectStone = _inputEventProvider.GetSelectStone();
            if (selectStone != null)
            {
                SelectStoneIndex = selectStone.Index;
            }
        }

        public override bool IsInputPlayer()
        {
            return true;
        }
    }
}
