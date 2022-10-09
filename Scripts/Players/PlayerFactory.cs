using System;
using Reversi.Players.AI;
using Reversi.Players.Input;
using Reversi.Services;
using Reversi.Stones.Stone;
using UnityEngine;

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
        /// <param name="createParent">作成する親オブジェクト</param>
        /// <returns>作成したプレイヤー</returns>
        public IPlayer CreatePlayer(PlayerType playerType, StoneState stoneState, Action<StoneState, int, int> putStoneAction, Transform createParent)
        {
            IPlayer player = null;
            switch (playerType)
            {
                case PlayerType.InputPlayer:
                    player = new InputPlayer(stoneState, putStoneAction, _inputEventProvider);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/Human"), createParent);
                    break;
                // ----- 本番で使用しないAI -----
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
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MLAgentAIPlayer"));
                    break;
                case PlayerType.MiniMonteKillerAIPlayer:
                    player = new MlAgentsAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MiniMonteKillerAIPlayer"));
                    break;
                case PlayerType.MlAgentAIPlayerLearn1:
                    player = new MlAgentsAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MLAgentAIPlayerLearn1"));
                    break;
                case PlayerType.MlAgentAIPlayerLearn2:
                    player = new MlAgentsAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MLAgentAIPlayerLearn2"));
                    break;
                // ----- 本番用、進捗に影響しないAI -----
                case PlayerType.MiniMaxAIRobotPlayer:
                    player = new MiniMaxAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/AIRobot"), createParent);
                    break;
                case PlayerType.MonteCarloAIRobotPlayer:
                    player = new MonteCarloAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/AIRobot"), createParent);
                    break;
                case PlayerType.MlAgentAIRobotPlayer:
                    player = new MlAgentsAIPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/AIRobot"), createParent);
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MLAgentAIPlayer")); // 強化学習（強）
                    break;
                // ----- 本番用、進捗に影響するAI -----
                case PlayerType.PikaruPlayer:
                    player = new PikaruPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/Pikaru"), createParent);
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MLAgentWeakAIPlayer")); // 強化学習（弱）
                    break;
                case PlayerType.MichaelPlayer:
                    player = new MichaelPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/Michael"), createParent);
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MLAgentAIPlayer")); // 強化学習（強）
                    break;
                case PlayerType.ElekiBearPlayer:
                    player = new ElekiBearPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/ElekiBear"), createParent);
                    break;
                case PlayerType.GoloyanPlayer:
                    player = new GoloyanPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/Goloyan"), createParent);
                    player.OnInstantiateAgent(_assetsService.LoadAssets("Player/MLAgentAIPlayer")); // 強化学習（MiniMonteキラー）
                    break;
                case PlayerType.ZeroPlayer:
                    player = new ZeroPlayer(stoneState, putStoneAction);
                    player.OnInstantiate(_assetsService.LoadAssets("FbxVariant/Zero"), createParent);
                    break;
            }
            return player;
        }
    }
}
