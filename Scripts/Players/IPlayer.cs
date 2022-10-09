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

        /// <summary>
        /// プレイヤー種類
        /// </summary>
        public PlayerType MyPlayerType { get; }

        public bool IsWaitSelect { set; get; }

        public void OnInitialize(PlayerType playerType, bool isDisplayAnimation);
        public void OnStartTurn(StoneState[,] stoneStates);
        public void OnUpdateTurn();
        public void OnEndGame(PlayerResultState resultState);
        public bool IsInputPlayer();
        public void SetEmotionParameter(float emotion);
        public void StartPutAnimation();
        public void StartResultAnimation(PlayerResultState state);
        public void OnInstantiate(GameObject obj, Transform parent = null);
        public void OnInstantiateAgent(GameObject obj, Transform parent = null);
        public void OnDestroy();
    }
}
