using System;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Players.AI
{
    /// <summary>
    /// 探索処理や評価値計算のアルゴリズム
    /// </summary>
    public static class AIAlgorithm
    {
        /// <summary>
        /// NegaMaxアルゴリズムによるストーンの探索
        /// </summary>
        /// <param name="stoneStates">ストーン状態配列</param>
        /// <param name="putStoneState">置くストーン状態</param>
        /// <param name="depth">探索する深さ</param>
        /// <param name="isRandom">スコアが同じだった場合にランダムにするか？</param>
        /// <returns>探索したストーン</returns>
        public static StoneIndex SearchNegaMaxStone(StoneState[,] stoneStates, StoneState putStoneState, int depth, bool isRandom = false)
        {
            // 探索したストーン
            StoneIndex resultStoneIndex = null;

            // 置くことが可能なストーンを全て調べる
            var maxScore = int.MinValue;
            var canPutStonesIndex = StoneCalculator.GetAllCanPutStonesIndex(stoneStates, putStoneState);
            foreach (var putStoneIndex in canPutStonesIndex)
            {
                // 次の階層の状態を調べる
                var putStoneStates = StoneCalculator.GetPutStoneState(stoneStates, putStoneState, putStoneIndex.X, putStoneIndex.Z);
                var score = -1 * GetNegaMaxScore(putStoneStates, GetReverseStoneState(putStoneState), depth - 1);

                // 最大スコアの場合、スコアと該当インデックスを保持
                if (maxScore < score)
                {
                    maxScore = score;
                    resultStoneIndex = putStoneIndex;
                }
                else if (isRandom && maxScore == score && UnityEngine.Random.Range(0, 2) == 0)
                {
                    // スコアが同じだったら1/2の確率で入れ替える
                    maxScore = score;
                    resultStoneIndex = putStoneIndex;
                }
            }
            return resultStoneIndex;
        }

        /// <summary>
        /// NegaMaxアルゴリズムによるスコアの計算
        /// </summary>
        /// <param name="stoneStates">ストーン状態の配列</param>
        /// <param name="putStoneState">置くストーン状態</param>
        /// <param name="depth">探索する深さ</param>
        /// <param name="isPrevPassed">上の階層でパスしたか？</param>
        /// <returns>指定階層の最大スコア</returns>
        private static int GetNegaMaxScore(StoneState[,] stoneStates, StoneState putStoneState, int depth, bool isPrevPassed = false)
        {
            // 葉ノードで評価関数を実行
            if (depth == 0) return EvaluateStoneStates(stoneStates, putStoneState);

            // 置くことが可能なストーンを全て調べる
            var maxScore = int.MinValue;
            var canPutStonesIndex = StoneCalculator.GetAllCanPutStonesIndex(stoneStates, putStoneState);
            foreach (var putStoneIndex in canPutStonesIndex)
            {
                // 次の階層の状態を調べる
                var putStoneStates = StoneCalculator.GetPutStoneState(stoneStates, putStoneState, putStoneIndex.X, putStoneIndex.Z);
                maxScore = Math.Max(maxScore, -1 * GetNegaMaxScore(putStoneStates, GetReverseStoneState(putStoneState), depth - 1));
            }

            // 見つからなかった場合
            if (maxScore == int.MinValue)
            {
                // ２回連続パスの場合、評価関数を実行
                if (isPrevPassed) return EvaluateStoneStates(stoneStates, putStoneState);
                // ストーン状態はそのままで、次の階層の状態を調べる
                return -1 * GetNegaMaxScore(stoneStates, GetReverseStoneState(putStoneState), depth - 1, true);
            }
            return maxScore;
        }

        /// <summary>
        /// NegaAlphaアルゴリズムによるストーンの探索
        /// </summary>
        /// <param name="stoneStates">ストーン状態の配列</param>
        /// <param name="putStoneState">置くストーン状態</param>
        /// <param name="depth">探索する深さ</param>
        /// <param name="isRandom">スコアが同じだった場合にランダムにするか？</param>
        /// <returns>探索したストーン</returns>
        public static StoneIndex SearchNegaAlphaStone(StoneState[,] stoneStates, StoneState putStoneState, int depth, bool isRandom = false)
        {
            // 探索したストーン
            StoneIndex resultStoneIndex = null;

            // 置くことが可能なストーンを全て調べる
            var alpha = int.MinValue + 1; // MinValueを反転するとintの範囲を超えてしまうため、+1する
            var beta = int.MaxValue;
            var canPutStonesIndex = StoneCalculator.GetAllCanPutStonesIndex(stoneStates, putStoneState);
            foreach (var putStoneIndex in canPutStonesIndex)
            {
                // 次の階層の状態を調べる
                var putStoneStates = StoneCalculator.GetPutStoneState(stoneStates, putStoneState, putStoneIndex.X, putStoneIndex.Z);
                var score = -1 * GetNegaAlphaScore(putStoneStates, GetReverseStoneState(putStoneState), depth - 1, -beta, -alpha);

                // 最大スコアの場合、スコアと該当インデックスを保持
                if (alpha < score)
                {
                    alpha = score;
                    resultStoneIndex = putStoneIndex;
                }
                else if (isRandom && alpha == score && UnityEngine.Random.Range(0, 2) == 0)
                {
                    // スコアが同じだったら1/2の確率で入れ替える
                    alpha = score;
                    resultStoneIndex = putStoneIndex;
                }
            }
            return resultStoneIndex;
        }

        /// <summary>
        /// NegaAlphaアルゴリズムによるスコアの計算
        /// </summary>
        /// <param name="stoneStates">ストーン状態の配列</param>
        /// <param name="putStoneState">置くストーン状態</param>
        /// <param name="depth">探索する深さ</param>
        /// <param name="alpha">探索範囲の下限</param>
        /// <param name="beta">探索範囲の上限</param>
        /// <param name="isPrevPassed">上の階層でパスしたか？</param>
        /// <returns>指定階層の最大スコア</returns>
        private static int GetNegaAlphaScore(StoneState[,] stoneStates, StoneState putStoneState, int depth, int alpha, int beta, bool isPrevPassed = false)
        {
            // 葉ノードで評価関数を実行
            if (depth == 0) return EvaluateStoneStates(stoneStates, putStoneState);

            // 置くことが可能なストーンを全て調べる
            var maxScore = int.MinValue;
            var canPutStonesIndex = StoneCalculator.GetAllCanPutStonesIndex(stoneStates, putStoneState);
            foreach (var putStoneIndex in canPutStonesIndex)
            {
                // 次の階層の状態を調べる
                var putStoneStates = StoneCalculator.GetPutStoneState(stoneStates, putStoneState, putStoneIndex.X, putStoneIndex.Z);
                var score = -1 * GetNegaAlphaScore(putStoneStates, GetReverseStoneState(putStoneState), depth - 1, -beta, -alpha);

                // NegaMax値が探索範囲の上限より上の場合は枝狩り
                if (score >= beta) return score;

                // alpha値、maxScoreを更新
                alpha = Math.Max(alpha, score);
                maxScore = Math.Max(maxScore, score);
            }

            // 見つからなかった場合
            if (maxScore == int.MinValue)
            {
                // ２回連続パスの場合、評価関数を実行
                if (isPrevPassed) return EvaluateStoneStates(stoneStates, putStoneState);
                // ストーン状態はそのままで、次の階層の状態を調べる
                return -1 * GetNegaAlphaScore(stoneStates, GetReverseStoneState(putStoneState), depth - 1, -beta, -alpha, true);
            }
            return maxScore;
        }

        /// <summary>
        /// ストーン状態を黒←→白で切り替えて返却
        /// </summary>
        private static StoneState GetReverseStoneState(StoneState stoneState)
        {
            return stoneState == StoneState.Black ? StoneState.White : StoneState.Black;
        }

        /// <summary>
        /// ストーン状態に対する評価値
        /// </summary>
        private static readonly int[,] EvaluateStoneStatesScore= new[,] {
            {  30, -12,   0,  -1,  -1,   0, -12,  30},
            { -12, -15,  -3,  -3,  -3,  -3, -15, -12},
            {   0,  -3,   0,  -1,  -1,   0,  -3,   0},
            {  -1,  -3,  -1,  -1,  -1,  -1,  -3,  -1},
            {  -1,  -3,  -1,  -1,  -1,  -1,  -3,  -1},
            {   0,  -3,   0,  -1,  -1,   0,  -3,   0},
            { -12, -15,  -3,  -3,  -3,  -3, -15, -12},
            {  30, -12,   0,  -1,  -1,   0, -12,  30},
        };

        /// <summary>
        /// ストーン状態に対する評価値の計算
        /// </summary>
        /// <param name="stoneStates">ストーン状態配列</param>
        /// <param name="putStoneState">置くストーン状態</param>
        /// <returns>ストーン状態に対する評価値</returns>
        public static int EvaluateStoneStates(StoneState[,] stoneStates, StoneState putStoneState)
        {
            // 白と黒のストーン位置からそれぞれスコアを求める
            var whiteScore = 0;
            var blackScore = 0;
            for (var x = 0; x < stoneStates.GetLength(0); x++)
            {
                for (var z = 0; z < stoneStates.GetLength(1); z++)
                {
                    var score = EvaluateStoneStatesScore[x, z];
                    if (stoneStates[x, z] == StoneState.White)
                    {
                        whiteScore += score;
                    }
                    else if (stoneStates[x, z] == StoneState.Black)
                    {
                        blackScore += score;
                    }
                }
            }
            // 自分の色のスコア - 相手の色のスコア
            if (putStoneState == StoneState.White)
            {
                return whiteScore - blackScore;
            }
            return blackScore - whiteScore;
        }
    }
}
