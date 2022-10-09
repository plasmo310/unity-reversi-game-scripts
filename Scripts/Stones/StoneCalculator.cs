using System;
using System.Collections.Generic;
using Reversi.Stones.Stone;
using UnityEngine;

namespace Reversi.Stones
{
    /// <summary>
    /// ストーンの計算やチェック処理を行う
    /// </summary>
    public static class StoneCalculator
    {
        /// <summary>
        /// ストーン状態を黒←→白で切り替えて返却
        /// </summary>
        public static StoneState GetReverseStoneState(StoneState stoneState)
        {
            return stoneState == StoneState.Black ? StoneState.White : StoneState.Black;
        }

        /// <summary>
        /// 各色のストーンの数を調べる
        /// </summary>
        /// <param name="checkStoneStates"></param>
        /// <param name="callback"></param>
        public static void CheckStoneColorCount(StoneState[,] checkStoneStates,
            Action<int, int> callback)
        {
            // ストーンの白、黒の数を調べる
            var whiteCount = 0;
            var blackCount = 0;
            for (var x = 0; x < checkStoneStates.GetLength(0); x++)
            {
                for (var z = 0; z < checkStoneStates.GetLength(1); z++)
                {
                    if (checkStoneStates[x, z] == StoneState.White) whiteCount++;
                    if (checkStoneStates[x, z] == StoneState.Black) blackCount++;
                }
            }
            // コールバックに渡す
            callback(whiteCount, blackCount);
        }

        /// <summary>
        /// 勝った方のストーン状態を返却する
        /// </summary>
        /// <param name="checkStoneStates"></param>
        /// <returns></returns>
        public static StoneState GetWinStoneState(StoneState[,] checkStoneStates)
        {
            var winStoneState = StoneState.Empty;
            CheckStoneColorCount(checkStoneStates, (whiteCount, blackCount) =>
            {
                if (whiteCount > blackCount)
                {
                    winStoneState = StoneState.White;
                }
                else if (blackCount > whiteCount)
                {
                    winStoneState = StoneState.Black;
                }
                else
                {
                    winStoneState = StoneState.Empty; // 引き分けの場合はEmptyで返す
                }
            });
            return winStoneState;
        }

        /// <summary>
        /// ゲームの進捗状態(0.0〜1.0)を返す
        /// </summary>
        /// <param name="stoneStates"></param>
        /// <returns></returns>
        public static float GetGameRate(StoneState[,] stoneStates)
        {
            var putCount = 0;
            foreach (var stoneState in stoneStates)
            {
                if (stoneState != StoneState.Empty) putCount++;
            }
            return (float) putCount / stoneStates.Length;
        }

        /// <summary>
        /// 置いた後のストーン状態を返却する
        /// </summary>
        /// <param name="stoneStates"></param>
        /// <param name="putState"></param>
        /// <param name="putX"></param>
        /// <param name="putZ"></param>
        /// <param name="turnStonesIndex"></param>
        /// <returns>置いた後のストーン状態</returns>
        public static StoneState[,] GetPutStoneState(StoneState[,] stoneStates, StoneState putState, int putX, int putZ, List<StoneIndex> turnStonesIndex = null)
        {
            // 既に置いてある場合、そのまま返す
            if (stoneStates == null || stoneStates[putX, putZ] != StoneState.Empty) return stoneStates;

            // ひっくり返せるストーンが指定されていない場合、取得する
            turnStonesIndex ??= GetTurnStonesIndex(stoneStates, putState, putX, putZ);
            if (turnStonesIndex.Count == 0) return stoneStates;

            // 引数からクローンを作成
            var putStoneStates = stoneStates.Clone() as StoneState[,];

            // ストーンを置く
            if (putStoneStates != null)
            {
                putStoneStates[putX, putZ] = putState;

                // ひっくり返す
                foreach (var tuneStone in turnStonesIndex)
                {
                    putStoneStates[tuneStone.X, tuneStone.Z] = putState;
                }
            }
            return putStoneStates;
        }

        /// <summary>
        /// 置くことが可能なストーン状態配列を返却する
        /// </summary>
        /// <param name="stoneStates"></param>
        /// <param name="putState"></param>
        /// <returns> 置くことが可能なストーン状態配列</returns>
        public static List<StoneIndex> GetAllCanPutStonesIndex(StoneState[,] stoneStates, StoneState putState)
        {
            var canPutStones = new List<StoneIndex>();
            for (var x = 0; x < stoneStates.GetLength(0); x++)
            {
                for (var z = 0; z < stoneStates.GetLength(1); z++)
                {
                    // 置けるストーンなら追加
                    if (GetTurnStonesIndex(stoneStates, putState, x, z).Count > 0)
                    {
                        canPutStones.Add(new StoneIndex(x, z));
                    }
                }
            }
            return canPutStones;
        }

        /// <summary>
        /// 置いた位置からひっくり返せるストーン情報を返却する
        /// </summary>
        /// <param name="stoneStates">チェックするストーン配列</param>
        /// <param name="putState">置いた色</param>
        /// <param name="putX">置いたX位置</param>
        /// <param name="putZ">置いたZ位置</param>
        /// <returns>ひっくり返せるストーン情報(Index)</returns>
        public static List<StoneIndex> GetTurnStonesIndex(StoneState[,] stoneStates, StoneState putState, int putX, int putZ)
        {
            // 既にストーンが置かれていたら空で返却
            var turnStonesIndex = new List<StoneIndex>();
            if (stoneStates == null || stoneStates[putX, putZ] != StoneState.Empty) return turnStonesIndex;

            // 8方向分のストーンを調べて返却する
            foreach (var searchVec in SearchAllVectors)
            {
                turnStonesIndex.AddRange(GetTurnStonesIndex(stoneStates, putState, putX, putZ, searchVec));
            }
            return turnStonesIndex;
        }

        /// <summary>
        /// 置いた位置から指定方向にひっくり返せるストーン情報を返却する
        /// </summary>
        /// <param name="stoneStates">チェックするストーン配列</param>
        /// <param name="putState">置いた色</param>
        /// <param name="putX">置いたX位置</param>
        /// <param name="putZ">置いたZ位置</param>
        /// <param name="searchVec">調べる方向(-1 or 0 or 1)</param>
        /// <returns>ひっくり返せるストーン情報(Index)</returns>
        private static List<StoneIndex> GetTurnStonesIndex(StoneState[,] stoneStates, StoneState putState, int putX, int putZ, SearchVector searchVec)
        {
            // 入力値チェック
            if (putState != StoneState.Black && putState != StoneState.White)
            {
                Debug.LogError("*** StoneManager.GetTuneStones valid request!!");
                return new List<StoneIndex>();
            }

            // 置く色と反対の色をターゲットにする
            var targetState = putState == StoneState.Black ? StoneState.White : StoneState.Black;

            // ひっくり返せるストーンを調べる
            var turnStonesIndex = new List<StoneIndex>();
            var x = putX;
            var z = putZ;
            while (true)
            {
                // 調べる方向に進む
                x += searchVec.X;
                z += searchVec.Z;

                // indexが範囲外になったら調査終了
                if (x < 0 || stoneStates.GetLength(0) <= x ||
                    z < 0 || stoneStates.GetLength(1) <= z)
                {
                    break;
                }
                // 何も置いていなければ調査終了
                var stoneState = stoneStates[x, z];
                if (stoneState == StoneState.Empty)
                {
                    break;
                }

                // 置いたストーンと同じ色の場合、調査したストーンを返却する
                if (stoneState == putState)
                {
                    return turnStonesIndex;
                }
                // ターゲットの色だったらリストに追加
                if (stoneState == targetState)
                {
                    turnStonesIndex.Add(new StoneIndex(x, z));
                }
            }
            // 見つからなかったら空で返却
            return new List<StoneIndex>();
        }

        /// <summary>
        /// 探索方向
        /// </summary>
        private static readonly SearchVector[] SearchAllVectors = new SearchVector[]
        {
            new (1,0),   // left
            new (1,1),   // left up diagonal
            new (1,-1),  // left down diagonal
            new (-1,0),  // right
            new (-1,1),  // right up diagonal
            new (-1,-1), // right down diagonal
            new (0,1),   // up
            new (0,-1),  // down
        };
        private class SearchVector
        {
            public int X { get; private set; }
            public int Z { get; private set; }
            public SearchVector(int x, int z)
            {
                // 値をチェックする
                var isValid = false;
                isValid = isValid || (x != 0 && x != 1 && x != -1);
                isValid = isValid || (z != 0 && z != 1 && z != -1);
                isValid = isValid || (x == 0 && z == 0);
                if (isValid) throw new ArgumentException("SearchVectorには0,1,-1のみ指定できます。((0,0)は除く)");

                this.X = x;
                this.Z = z;
            }
        }
    }
}
