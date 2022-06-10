using Reversi.Stones.Stone;

namespace Reversi.Players.Input
{
    public interface IInputEventProvider
    {
        public StoneBehaviour GetSelectStone();
    }
}
