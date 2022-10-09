using System;
using Reversi.Managers;
using Reversi.Stones.Stone;
using Reversi.UIs.View;
using UnityEngine;
using VContainer;

namespace Reversi.UIs.Presenter
{
    /// <summary>
    /// デバッグ用のチートPresenter
    /// 初期状態にボタンを押下すると一瞬で盤面を切り替える
    /// Prefabを置いてGameLifeTimeScopeにアタッチすることで有効になる
    /// </summary>
    public class DebugGamePresenter : MonoBehaviour
    {
        [SerializeField] private DebugGameView _view;

        private StoneManager _stoneManager;

        [Inject]
        public void Construct(StoneManager stoneManager)
        {
            _stoneManager = stoneManager;
        }

        private void Start()
        {
            // 勝ち確
            _view.SetListenerWinButton(() =>
            {
                var stoneStates = ConvertNumbersToStoneStates(new[,] {
                    { 1, 1, 1, 1, 1, 1, 1, 1},
                    { 1, 1, 1, 1, 1, 1, 1, 1},
                    { 1, 1, 1, 1, 1, 1, 1, 1},
                    { 1, 1, 1, 1, 2, 1, 1, 1},
                    { 1, 1, 1, 2, 1, 1, 1, 1},
                    { 1, 1, 1, 1, 1, 1, 1, 1},
                    { 1, 1, 1, 1, 1, 1, 1, 2},
                    { 1, 1, 1, 1, 1, 1, 1, 0},
                });
                _stoneManager.SetForceStoneStatesDebug(stoneStates);
                _stoneManager.UpdateAllViewStones(stoneStates);
                _stoneManager.ReleaseFocusStones();
                _stoneManager.SetFocusAllCanPutStones(StoneState.White);
            });
            // 負け確
            _view.SetListenerLoseButton(() =>
            {
                var stoneStates = ConvertNumbersToStoneStates(new[,] {
                    { 2, 2, 2, 2, 2, 2, 2, 2},
                    { 2, 2, 2, 2, 2, 2, 2, 2},
                    { 2, 2, 2, 2, 2, 2, 2, 2},
                    { 2, 2, 2, 1, 2, 2, 2, 2},
                    { 2, 2, 2, 2, 1, 2, 2, 2},
                    { 2, 2, 2, 2, 2, 2, 2, 1},
                    { 2, 2, 2, 2, 2, 2, 2, 2},
                    { 2, 2, 2, 2, 2, 2, 2, 0},
                });
                _stoneManager.SetForceStoneStatesDebug(stoneStates);
                _stoneManager.UpdateAllViewStones(stoneStates);
                _stoneManager.ReleaseFocusStones();
                _stoneManager.SetFocusAllCanPutStones(StoneState.White);
            });
            // 引き分け
            _view.SetListenerDrawButton(() =>
            {
                var stoneStates = ConvertNumbersToStoneStates(new[,] {
                    { 2, 2, 2, 2, 2, 2, 2, 2},
                    { 2, 2, 2, 2, 2, 2, 2, 2},
                    { 2, 2, 2, 2, 2, 2, 2, 2},
                    { 2, 2, 2, 1, 2, 2, 2, 2},
                    { 1, 1, 1, 2, 1, 1, 1, 1},
                    { 1, 1, 1, 1, 1, 1, 1, 1},
                    { 1, 1, 1, 1, 1, 1, 1, 2},
                    { 1, 1, 1, 1, 1, 1, 1, 0},
                });
                _stoneManager.SetForceStoneStatesDebug(stoneStates);
                _stoneManager.UpdateAllViewStones(stoneStates);
                _stoneManager.ReleaseFocusStones();
                _stoneManager.SetFocusAllCanPutStones(StoneState.White);
            });
        }

        private static StoneState[,] ConvertNumbersToStoneStates(int[,] stoneNumbers)
        {
            var stoneStates = new StoneState[stoneNumbers.GetLength(0), stoneNumbers.GetLength(1)];
            for (var x = 0; x < stoneNumbers.GetLength(0); x++)
            {
                for (var z = 0; z < stoneNumbers.GetLength(1); z++)
                {
                    stoneStates[x, z] = (StoneState) Enum.ToObject(typeof(StoneState), stoneNumbers[x, z]);
                }
            }
            return stoneStates;
        }
    }
}
