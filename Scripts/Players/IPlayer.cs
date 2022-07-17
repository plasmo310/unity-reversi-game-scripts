using Reversi.Managers;
using Reversi.Stones.Stone;
using UnityEngine;

namespace Reversi.Players
{
    public interface IPlayer
    {
        /// <summary>
        /// 自分のストーン状態(黒or白)
        /// </summary>
        public StoneState MyStoneState { get; }

        public bool IsWaitSelect { set; get; }

        public void OnStartTurn(StoneState[,] stoneStates);
        public void OnUpdateTurn();
        public void OnEndGame(PlayerResultState resultState);
        public bool IsInputPlayer();
        public void OnInstantiate(GameObject obj);
        public void OnDestroy();
    }
}
