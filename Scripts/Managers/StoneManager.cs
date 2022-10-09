using System.Collections.Generic;
using Reversi.Audio;
using Reversi.Board;
using Reversi.Services;
using Reversi.Settings;
using Reversi.Stones;
using Reversi.Stones.Stone;
using UniRx;
using UnityEngine;
using VContainer;

namespace Reversi.Managers
{
    /// <summary>
    /// ストーン管理クラス
    /// </summary>
    public class StoneManager
    {
        private readonly GameSettings _gameSettings;
        private readonly BoardBehaviour _boardBehaviour;
        private readonly IAudioService _audioService;

        /// <summary>
        /// 各色ごとのストーンの数
        /// </summary>
        private readonly ReactiveProperty<int> _whiteStoneCount = new(0);
        private readonly ReactiveProperty<int> _blackStoneCount = new(0);
        public IReadOnlyReactiveProperty<int> WhiteStoneCount => _whiteStoneCount;
        public IReadOnlyReactiveProperty<int> BlackStoneCount => _blackStoneCount;

        /// <summary>
        /// 土台となるオブジェクト
        /// </summary>
        private readonly GameObject _stonesBase;

        /// <summary>
        /// ストーンPrefab
        /// </summary>
        private readonly GameObject _stonePrefab;

        /// <summary>
        /// ストーン配列
        /// </summary>
        private StoneState[,] _stoneStates;
        private StoneBehaviour[,] _viewStoneBehaviours; // 表示用

        /// <summary>
        /// Y方向オフセット
        /// </summary>
        private static readonly float StoneOffsetY = 0.1f;

        [Inject]
        public StoneManager(IAssetsService assetsService, IAudioService audioService, GameSettings gameSettings, BoardBehaviour boardBehaviour)
        {
            _audioService = audioService;
            _gameSettings = gameSettings;
            _boardBehaviour = boardBehaviour;

            // 土台となるオブジェクトを生成
            _stonesBase = new GameObject("Stones");
            _stonesBase.transform.SetParent(_boardBehaviour.transform);
            _stonesBase.transform.position = _boardBehaviour.transform.position;
            _stonesBase.transform.localScale = Vector3.one;

            // Prefab読み込み
            _stonePrefab = assetsService.LoadAssets("Stone");
        }

        /// <summary>
        /// ストーン初期化
        /// </summary>
        public void InitializeStones()
        {
            // 一辺あたりのセル数
            var cellSideCount = BoardBehaviour.CellSideCount;

            // 作成済のストーンを削除
            foreach(Transform child in _stonesBase.transform){
                Object.Destroy(child.gameObject);
            }

            // ストーンをEmptyで初期化して設定
            _viewStoneBehaviours = new StoneBehaviour[cellSideCount, cellSideCount];
            _stoneStates = new StoneState[cellSideCount, cellSideCount];
            for (var x = 0; x < _viewStoneBehaviours.GetLength(0); x++)
            {
                for (var z = 0; z < _viewStoneBehaviours.GetLength(1); z++)
                {
                    var stone = UnityEngine.Object.Instantiate(_stonePrefab, _stonesBase.transform, true);
                    stone.transform.localPosition = _boardBehaviour.GetCellPosition(x, z) + Vector3.up * StoneOffsetY;
                    stone.transform.localScale = _stonePrefab.transform.localScale;
                    _viewStoneBehaviours[x, z] = stone.GetComponent<StoneBehaviour>();
                    _viewStoneBehaviours[x, z].SetIndex(x, z);
                    _viewStoneBehaviours[x, z].IsDisplayAnimation = _gameSettings.DebugOption.isDisplayAnimation;
                    _stoneStates[x, z] = StoneState.Empty;
                }
            }

            // 中央のストーン状態を変更
            var centerIndex1 = cellSideCount / 2;
            var centerIndex2 = centerIndex1 - 1;
            _stoneStates[centerIndex1, centerIndex1] = StoneState.White;
            _stoneStates[centerIndex2, centerIndex1] = StoneState.Black;
            _stoneStates[centerIndex1, centerIndex2] = StoneState.Black;
            _stoneStates[centerIndex2, centerIndex2] = StoneState.White;

            // ストーンの表示を更新
            UpdateAllViewStones(_stoneStates);
        }

        /// <summary>
        /// ストーンが置けるかどうか？
        /// </summary>
        public bool IsCanPutStone()
        {
            return GetAllCanPutStonesIndex(StoneState.Black).Count > 0
                   || GetAllCanPutStonesIndex(StoneState.White).Count > 0;
        }

        /// <summary>
        /// 置くことが可能なストーン情報を返却する
        /// </summary>
        /// <param name="putState"></param>
        /// <returns></returns>
        public List<StoneIndex> GetAllCanPutStonesIndex(StoneState putState)
        {
            return StoneCalculator.GetAllCanPutStonesIndex(_stoneStates, putState);
        }

        /// <summary>
        /// ストーンを置く
        /// </summary>
        /// <param name="putState"></param>
        /// <param name="putX"></param>
        /// <param name="putZ"></param>
        /// <returns>ストーンを置けたかどうか</returns>
        public bool PutStone(StoneState putState, int putX, int putZ)
        {
            // 置けなかった場合、falseを返却
            var turnStonesIndex = StoneCalculator.GetTurnStonesIndex(_stoneStates, putState, putX, putZ);
            if (turnStonesIndex == null || turnStonesIndex.Count == 0) return false;

            // SE再生
            _audioService.PlayOneShot(ReversiAudioType.SePut1);

            // ストーンを置いて状態を更新する
            _stoneStates = StoneCalculator.GetPutStoneState(_stoneStates, putState, putX, putZ, turnStonesIndex);

            // ストーンの表示を更新
            UpdateAllViewStones(_stoneStates, new StoneIndex(putX, putZ));

            return true;
        }

        /// <summary>
        /// 全てのストーンの表示を更新する
        /// </summary>
        public void UpdateAllViewStones(StoneState[,] stoneStates, StoneIndex putStoneIndex = null)
        {
            // ストーンオブジェクトの表示を更新
            for (var x = 0; x < _viewStoneBehaviours.GetLength(0); x++)
            {
                for (var z = 0; z < _viewStoneBehaviours.GetLength(1); z++)
                {
                    _viewStoneBehaviours[x, z].ChangeViewState(stoneStates[x, z], putStoneIndex);
                }
            }

            // ストーンの数も更新する
            StoneCalculator.CheckStoneColorCount(stoneStates, (whiteStoneCount, blackStoneCount) =>
            {
                _whiteStoneCount.Value = whiteStoneCount;
                _blackStoneCount.Value = blackStoneCount;
            });
        }

        /// <summary>
        /// プレイヤーのストーン状態に対する勝利判定を返却する
        /// </summary>
        /// <param name="playerStoneState">プレイヤーのストーン状態</param>
        public PlayerResultState GetPlayerResultState(StoneState playerStoneState)
        {
            var winStoneState = StoneCalculator.GetWinStoneState(_stoneStates);
            if (winStoneState == StoneState.Empty)
            {
                return PlayerResultState.Draw;
            }
            return winStoneState == playerStoneState ? PlayerResultState.Win : PlayerResultState.Lose;
        }

        /// <summary>
        /// 指定されたストーンの割合を返却する
        /// </summary>
        /// <param name="playerStoneState"></param>
        /// <returns></returns>
        public float GetStoneStateRate(StoneState playerStoneState)
        {
            var rate = 0.0f;
            StoneCalculator.CheckStoneColorCount(_stoneStates, (whiteCount, blackCount) =>
            {
                if (playerStoneState == StoneState.White)
                {
                    rate = (float) whiteCount / (whiteCount + blackCount);
                }
                else
                {
                    rate = (float) blackCount / (whiteCount + blackCount);
                }
            });
            return rate;
        }

        /// <summary>
        /// フォーカスしているストーン
        /// </summary>
        private List<StoneBehaviour> _focusStones;

        /// <summary>
        /// ストーンをフォーカスする
        /// </summary>
        private void SetFocusStones(List<StoneBehaviour> focusStones)
        {
            foreach (var focusStone in focusStones)
            {
                focusStone.SetIsFocus(true);
            }
            _focusStones = focusStones;
        }

        /// <summary>
        /// 置くことが可能な全てのストーンをフォーカスする
        /// </summary>
        /// <param name="putState">置く色</param>
        public void SetFocusAllCanPutStones(StoneState putState)
        {
            var canPutStones = new List<StoneBehaviour>();
            for (var x = 0; x < _viewStoneBehaviours.GetLength(0); x++)
            {
                for (var z = 0; z < _viewStoneBehaviours.GetLength(1); z++)
                {
                    // 置けるストーンなら追加
                    if (StoneCalculator.GetTurnStonesIndex(_stoneStates, putState, x, z).Count > 0)
                    {
                        canPutStones.Add(_viewStoneBehaviours[x, z]);
                    }
                }
            }
            SetFocusStones(canPutStones);
        }

        /// <summary>
        /// フォーカスしているストーンを解除する
        /// </summary>
        public void ReleaseFocusStones()
        {
            if (_focusStones == null) return;
            foreach (var focusStone in _focusStones)
            {
                focusStone.SetIsFocus(false);
            }
            _focusStones = null;
        }

        /// <summary>
        /// ストーン配列のクローンを返却する
        /// </summary>
        public StoneState[,] GetStoneStatesClone()
        {
            return _stoneStates.Clone() as StoneState[,];
        }

        /// <summary>
        /// StoneStatesの強制上書き（デバッグ用）
        /// </summary>
        /// <param name="stoneStates"></param>
        public void SetForceStoneStatesDebug(StoneState[,] stoneStates)
        {
            _stoneStates = stoneStates;
        }
    }
}
