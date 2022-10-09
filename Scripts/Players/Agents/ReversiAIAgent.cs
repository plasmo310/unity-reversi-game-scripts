using Cysharp.Threading.Tasks;
using Reversi.Managers;
using Reversi.Stones.Stone;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace Reversi.Players.Agents
{
    /// <summary>
    /// リバーシのAIエージェント
    /// 外部(MlAgentAIPlayer等)から呼び出す形で使用する
    /// </summary>
    public class ReversiAIAgent : Agent
    {
        /// <summary>
        /// エピソード開始
        /// </summary>
        public override void OnEpisodeBegin()
        {
            SetReward(0.0f);
        }

        /// <summary>
        /// 観察
        /// </summary>
        public override void CollectObservations(VectorSensor sensor)
        {
            if (_stoneStates == null) return;

            var learnStoneStates = ConvertLearnStoneStates(_stoneStates, _myStoneState);
            sensor.AddObservation(learnStoneStates);
        }

        /// <summary>
        /// ストーン状態配列を学習形式に変換して返却する
        /// </summary>
        private float[] ConvertLearnStoneStates(StoneState[,] stoneStates, StoneState myStoneState)
        {
            if (stoneStates == null) return null;

            var convStoneStates = new float[stoneStates.Length];
            for (var i = 0; i < stoneStates.Length; i++)
            {
                var x = i % stoneStates.GetLength(0);
                var z = i / stoneStates.GetLength(0);
                convStoneStates[i] = ConvertLearnStoneState(stoneStates[x, z], myStoneState);
            }
            return convStoneStates;
        }

        /// <summary>
        /// ストーン状態を学習形式に変換して返却する
        /// </summary>
        private float ConvertLearnStoneState(StoneState stoneState, StoneState myStoneState)
        {
            // 何も置いてない：0
            if (stoneState == StoneState.Empty) return 0.0f;
            // 自分の色：1、相手の色：2
            return stoneState == myStoneState ? 1.0f : 2.0f;
        }

        /// <summary>
        /// 行動
        /// </summary>
        public override void OnActionReceived(ActionBuffers actions)
        {
            // ストーンを置く
            var discreteActions = actions.DiscreteActions;
            var index = discreteActions[0];
            var x = index % _stoneStates.GetLength(0);
            var z = index / _stoneStates.GetLength(0);
            var selectIndex = new StoneIndex(x, z);

            // 何故かマスクが効かないことがあったので、置ける場所かどうかのチェックを行う
            var isExist = false;
            foreach (var canPutStoneIndex in _canPutStoneIndices)
            {
                if (canPutStoneIndex.Equals(selectIndex)) isExist = true;
            }
            // 置けない場所に置こうとしたら再度行動をリクエスト
            if (!isExist)
            {
                RequestDecision();
                return;
            }
            _selectStoneIndex = selectIndex;
        }

        /// <summary>
        /// 行動のマスク
        /// </summary>
        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            // 置くことが可能なストーンを調べる
            for (var i = 0; i < _stoneStates.Length; i++)
            {
                var x = i % _stoneStates.GetLength(0);
                var z = i / _stoneStates.GetLength(0);
                var isCanPutStone = false;
                foreach (var canPutStoneIndex in _canPutStoneIndices)
                {
                    if (x == canPutStoneIndex.X && z == canPutStoneIndex.Z)
                    {
                        isCanPutStone = true;
                    }
                }
                // 置くことが可能なストーンのみ活性にする
                actionMask.SetActionEnabled(0, i, isCanPutStone);
            }
        }

        /// <summary>
        /// ヒューリスティック処理
        /// </summary>
        /// <param name="actionsOut"></param>
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            // 置けるストーンの中からランダムで決定する
            var stoneIndex = _canPutStoneIndices[UnityEngine.Random.Range(0, _canPutStoneIndices.Length)];
            var index = stoneIndex.Z * _stoneStates.GetLength(0) + stoneIndex.X;
            var discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = index;
        }

        /// <summary>
        /// ストーン情報
        /// </summary>
        private StoneState[,] _stoneStates;
        private StoneIndex[] _canPutStoneIndices;
        private StoneState _myStoneState;
        private StoneIndex _selectStoneIndex;

        /// <summary>
        /// ストーン探索処理
        /// </summary>
        /// <param name="stoneStates">ストーン状態配列</param>
        /// <param name="canPutStoneIndices">置けるマスのインデックス配列</param>
        /// <param name="myStoneState">自分のストーンの色</param>
        /// <returns>探索結果(置くインデックス)</returns>
        public async UniTask<StoneIndex> OnSearchSelectStone(StoneState[,] stoneStates, StoneIndex[] canPutStoneIndices, StoneState myStoneState)
        {
            // ストーン状態を設定
            _stoneStates = stoneStates;
            _canPutStoneIndices = canPutStoneIndices;
            _myStoneState = myStoneState;
            _selectStoneIndex = null;

            // 決定を要求する
            RequestDecision();

            // 置く位置を探索したら返却する
            await UniTask.WaitWhile(() => _selectStoneIndex == null);
            return _selectStoneIndex;
        }

        /// <summary>
        /// ゲーム終了処理
        /// </summary>
        /// <param name="resultState">勝敗結果</param>
        public void OnGameEnd(PlayerResultState resultState)
        {
            // 結果に応じて報酬を与える
            switch (resultState)
            {
                case PlayerResultState.Win:
                    SetReward(1.0f);
                    break;
                case PlayerResultState.Lose:
                    SetReward(-1.0f);
                    break;
            }
            // エピソード終了
            EndEpisode();
        }
    }
}
