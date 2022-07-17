using System;
using Reversi.Players.AI;
using Reversi.Players.Input;
using Reversi.Services;
using Reversi.Stones.Stone;

namespace Reversi.Players
{
    /// <summary>
    /// プレイヤー作成クラス
    /// </summary>
    public class PlayerFactory
    {
        private readonly IAssetsService _assetsService;
        private readonly IInputEventProvider _inputEventProvider;

        public PlayerFactory(IAssetsService assetsService, IInputEventProvider inputEventProvider)
        {
            _assetsService = assetsService;
            _inputEventProvider = inputEventProvider;
        }

        /// <summary>
        /// プレイヤー作成処理
        /// </summary>
        /// <param name="playerType">プレイヤータイプ</param>
        /// <param name="stoneState">プレイヤーの石の色</param>
        /// <param name="putStoneAction">ストーンを置く処理</param>
        /// <returns>作成したプレイヤー</returns>
        public IPlayer CreatePlayer(PlayerType playerType, StoneState stoneState, Action<StoneState, int, int> putStoneAction)
        {
            IPlayer player = null;
            switch (playerType)
            {
                case PlayerType.InputPlayer:
                    player = new InputPlayer(stoneState, putStoneAction, _inputEventProvider);
                    break;
                case PlayerType.RandomAIPlayer:
                    player = new RandomAIPlayer(stoneState, putStoneAction);
                    break;
                case PlayerType.MiniMaxAIPlayer:
                    player = new MiniMaxAIPlayer(stoneState, putStoneAction);
                    break;
                case PlayerType.MonteCarloAIPlayer:
                    player = new MonteCarloAIPlayer(stoneState, putStoneAction);
                    break;
                case PlayerType.MiniMaxMonteAIPlayer:
                    player = new MiniMaxMontePlayer(stoneState, putStoneAction);
                    break;
                case PlayerType.MlAgentAIPlayer:
                    player = new MlAgentsAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("Player/MLAgentAIPlayer"));
                    break;
                case PlayerType.MlAgentAIPlayerLearn1:
                    player = new MlAgentsAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("Player/MLAgentAIPlayerLearn1"));
                    break;
                case PlayerType.MlAgentAIPlayerLearn2:
                    player = new MlAgentsAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("Player/MLAgentAIPlayerLearn2"));
                    break;
            }
            return player;
        }
    }
}
