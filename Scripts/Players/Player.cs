using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Reversi.Managers;
using Reversi.Players.Agents;
using Reversi.Players.Animation;
using Reversi.Players.Display;
using Reversi.Stones.Stone;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Reversi.Players
{
    /// <summary>
    /// プレイヤー共通クラス
    /// </summary>
    public abstract class Player : IPlayer, IDisposable
    {
        /// <summary>
        /// 自分のストーン状態(黒or白)
        /// </summary>
        protected readonly StoneState MyStoneState;
        StoneState IPlayer.MyStoneState => MyStoneState;

        /// <summary>
        /// 自分のプレイヤー種類
        /// </summary>
        private PlayerType _playerType;
        PlayerType IPlayer.MyPlayerType => _playerType;

        /// <summary>
        /// ストーン選択処理を待機するか？
        /// </summary>
        private bool _isWaitSelect;
        bool IPlayer.IsWaitSelect
        {
            get => _isWaitSelect;
            set => _isWaitSelect = value;
        }

        /// <summary>
        /// ストーン配列(AIチェック用)
        /// </summary>
        protected StoneState[,] StoneStates;

        /// <summary>
        /// 選択したストーン
        /// 継承先のクラスでこの変数に設定する
        /// </summary>
        protected StoneIndex SelectStoneIndex;

        /// <summary>
        /// 感情値
        /// </summary>
        protected enum PlayerEmotion
        {
            Normal = 0, // 普通
            Heat = 1,   // 焦り
            Sad = -1,   // 悲しみ
        }
        protected PlayerEmotion Emotion;

        /// <summary>
        /// プレイヤーのゲームオブジェクト
        /// </summary>
        private GameObject _playerGameObject;
        protected ReversiAIAgent PlayerAIAgent;
        private PlayerAnimationBehaviour _playerAnimationBehaviour;

        /// <summary>
        /// ストーン選択処理
        /// </summary>
        private readonly Action<StoneState, int, int> _putStoneAction;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        protected readonly CancellationTokenSource CancellationTokenSource;

        protected Player(StoneState myStoneState, Action<StoneState, int, int> putStoneAction)
        {
            _putStoneAction = putStoneAction;
            MyStoneState = myStoneState;

            // キャンセルトークン発行
            CancellationTokenSource = new CancellationTokenSource();
        }

        public void Dispose()
        {
            CancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="playerType"></param>
        /// <param name="isDisplayAnimation"></param>
        public void OnInitialize(PlayerType playerType, bool isDisplayAnimation)
        {
            // アニメーションさせる場合のみDisplayPlayerBehaviourを初期化
            if (_playerGameObject == null || !isDisplayAnimation) return;

            _playerType = playerType;

            // DisplayPlayerBehaviourを取得
            _playerAnimationBehaviour = _playerGameObject.GetComponent<PlayerAnimationBehaviour>();

            // 初期化
            var playerDisplayBehaviour = _playerGameObject.GetComponentInParent<PlayerDisplayBehaviour>();
            if (playerDisplayBehaviour != null) playerDisplayBehaviour.Initialize(playerType);
        }

        /// <summary>
        /// ターン開始
        /// </summary>
        /// <param name="stoneStates"></param>
        public void OnStartTurn(StoneState[,] stoneStates)
        {
            // ストーン配列を設定して初期化
            StoneStates = stoneStates;
            SelectStoneIndex = null;

            // 思考開始
            StartThink();
        }

        /// <summary>
        /// ターン更新
        /// </summary>
        public void OnUpdateTurn()
        {
            // 思考更新
            UpdateThink();

            // ストーンが選択されたら置く
            if (SelectStoneIndex == null) return;
            _putStoneAction(MyStoneState, SelectStoneIndex.X, SelectStoneIndex.Z);
            SelectStoneIndex = null; // 初期化
        }

        /// <summary>
        /// 思考処理
        /// </summary>
        protected virtual void StartThink() { }
        protected virtual void UpdateThink() { }

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        public void OnEndGame(PlayerResultState resultState)
        {
            // Agentが設定されていればゲーム終了処理を呼ぶ
            if (PlayerAIAgent != null)
            {
                PlayerAIAgent.OnGameEnd(resultState);
            }
            EndGame(resultState);
        }
        protected virtual void EndGame(PlayerResultState resultState) { }

        /// <summary>
        /// ストーン選択待機処理
        /// </summary>
        protected async UniTask WaitSelectTime(int waitMs, CancellationToken token)
        {
            if (_isWaitSelect)
            {
                await UniTask.Delay(waitMs, cancellationToken: token);
            }
        }

        /// <summary>
        /// 入力プレイヤーかどうか？
        /// </summary>
        public virtual bool IsInputPlayer()
        {
            // 基本的にfalse
            return false;
        }

        /// <summary>
        /// 感情値を設定
        /// </summary>
        /// <param name="myStoneStateRate">自分のストーン比率</param>
        public void SetEmotionParameter(float myStoneStateRate)
        {
            if (_playerAnimationBehaviour == null) return;
            Emotion = PlayerEmotion.Normal;
            if (myStoneStateRate < 0.6f) Emotion = PlayerEmotion.Heat;  // やばい時
            if (myStoneStateRate < 0.3f) Emotion = PlayerEmotion.Sad; // 更にやばい時
            _playerAnimationBehaviour.SetEmotionInt((int) Emotion);
        }

        /// <summary>
        /// ストーンを置くアニメーション開始
        /// </summary>
        public void StartPutAnimation()
        {
            if (_playerAnimationBehaviour == null) return;
            _playerAnimationBehaviour.StartPutAnimation();
        }

        /// <summary>
        /// 結果アニメーション表示
        /// </summary>
        /// <param name="state">勝敗結果</param>
        public void StartResultAnimation(PlayerResultState state)
        {
            if (_playerAnimationBehaviour == null) return;

            // 勝利した場合のみ1を設定
            var result = state == PlayerResultState.Win ? 1 : -1;
            _playerAnimationBehaviour.SetResult(result);
        }

        /// <summary>
        /// ゲームオブジェクトを生成する
        /// </summary>
        public void OnInstantiate(GameObject obj, Transform parent = null)
        {
            // ゲームオブジェクト生成
            _playerGameObject = parent != null ? Object.Instantiate(obj, parent) : Object.Instantiate(obj);
        }

        /// <summary>
        /// 機械学習Agentを生成する
        /// </summary>
        public void OnInstantiateAgent(GameObject obj, Transform parent = null)
        {
            // ゲームオブジェクト生成してAgentを設定
            var agentObj = parent != null ? Object.Instantiate(obj, parent) : Object.Instantiate(obj);
            PlayerAIAgent = agentObj.GetComponent<ReversiAIAgent>();
        }

        /// <summary>
        /// オブジェクトを破棄する
        /// プレイヤー切り替えの際に呼び出す
        /// </summary>
        public void OnDestroy()
        {
            if (_playerGameObject != null)
            {
                Object.Destroy(_playerGameObject);
            }
        }
    }
}
