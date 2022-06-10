using Reversi.Stones.Stone;

namespace Reversi.Players
{
    public interface IPlayer
    {
        /// <summary>
        /// 自分のストーン状態(黒or白)
        /// </summary>
        public StoneState MyStoneState { get; }

        public void StartTurn(StoneState[,] stoneStates);
        public void UpdateTurn();
        public bool IsInputPlayer();
    }
}
