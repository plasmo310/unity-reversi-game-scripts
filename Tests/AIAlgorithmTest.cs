using System;
using NUnit.Framework;
using Reversi.Players.AI;
using Reversi.Stones.Stone;

namespace Reversi.Tests
{
    /// <summary>
    /// ストーン関連アルゴリズムのテスト
    /// </summary>
    public class AIAlgorithmTest
    {
        [Test]
        public void T01_ストーン状態に対する評価値計算ができる()
        {
            // 初期状態
            var stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 1, 2, 0, 0, 0},
                { 0, 0, 0, 2, 1, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            // 白: (-2) - (-2) = 0
            var score = AIAlgorithm.EvaluateStoneStates(stoneStates, StoneState.White);
            Assert.AreEqual(0, score);
            // 黒: (-2) - (-2) = 0
            score = AIAlgorithm.EvaluateStoneStates(stoneStates, StoneState.Black);
            Assert.AreEqual(0, score);

            // 進んだ状態
            stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 2, 0, 0, 0, 0, 0, 0, 0},
                { 0, 2, 0, 0, 0, 0, 0, 0},
                { 0, 0, 2, 0, 0, 0, 0, 0},
                { 0, 0, 0, 2, 0, 0, 0, 0},
                { 0, 1, 1, 1, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            // 白: (-3 + -1 + -1) - (30 + -15 + 0 + -1) = -19
            score = AIAlgorithm.EvaluateStoneStates(stoneStates, StoneState.White);
            Assert.AreEqual(-19, score);
            // 黒: (30 + -15 + 0 + -1) - (-3 + -1 + -1) = 19
            score = AIAlgorithm.EvaluateStoneStates(stoneStates, StoneState.Black);
            Assert.AreEqual(19, score);
        }

        [Test]
        public void T02_NegaMaxアルゴリズムによりストーンの探索ができる()
        {
            AssertMiniMaxResult(AIAlgorithm.SearchNegaMaxStone);
        }

        [Test]
        public void T03_NegaAlphaアルゴリズムによりストーンの探索ができる()
        {
            AssertMiniMaxResult(AIAlgorithm.SearchNegaAlphaStone);
        }

        /// <summary>
        /// MiniMaxアルゴリズムによる探索結果の確認
        /// NegaMax、NegaAlphaでも同じ結果になる
        /// </summary>
        /// <param name="searchStoneIndex">ストーン探索処理</param>
        private void AssertMiniMaxResult(Func<StoneState[,], StoneState, int, bool, StoneIndex> searchStoneIndex)
        {
            // 初期状態
            var stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 1, 2, 0, 0, 0},
                { 0, 0, 0, 2, 1, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            // depth:1の場合 全て同じスコアのため最初に探索されたIndexが選択される
            // [1] -3: 2, 4
            // [1] -3: 3, 5
            // [1] -3: 4, 2
            // [1] -3: 5, 3
            var selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 1, false);
            Assert.AreEqual(new StoneIndex(2, 4), selectStoneIndex);
            // depth:2,3でも同様
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 2, false);
            Assert.AreEqual(new StoneIndex(2, 4), selectStoneIndex);
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 3, false);
            Assert.AreEqual(new StoneIndex(2, 4), selectStoneIndex);

            // 角が取れる場合
            stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 2, 1, 0, 0, 0, 0, 0},
                { 0, 2, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            // 角が選択されること
            // [1] 12: 5, 0
            // [1] 18: 7, 0
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 1, false);
            Assert.AreEqual(new StoneIndex(7, 0), selectStoneIndex);
            // depth:2,3でも同様
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 2, false);
            Assert.AreEqual(new StoneIndex(7, 0), selectStoneIndex);
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 3, false);
            Assert.AreEqual(new StoneIndex(7, 0), selectStoneIndex);

            // 角は取れるが微妙な場合
            stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 1, 0, 0, 0, 0, 0, 0},
                { 0, 0, 2, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 1, 2, 1, 0, 0, 0, 0, 0},
                { 2, 2, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            // depth:1では相手の角を防いだ方が得策だと判断する
            // [1] 14: 3, 3
            // [1] -9: 7, 0
            // [1] -15: 7, 2
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 1, false);
            Assert.AreEqual(new StoneIndex(3, 3), selectStoneIndex);
            // depth:2,3でも同様
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 2, false);
            Assert.AreEqual(new StoneIndex(3, 3), selectStoneIndex);
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 3, false);
            Assert.AreEqual(new StoneIndex(3, 3), selectStoneIndex);

            // ストーンが置けなくなる場合
            stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 1, 2, 2, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            // エラーにならないこと
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 1, false);
            Assert.AreEqual(new StoneIndex(3, 5), selectStoneIndex);
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 2, false);
            Assert.AreEqual(new StoneIndex(3, 5), selectStoneIndex);
            selectStoneIndex = searchStoneIndex(stoneStates, StoneState.White, 3, false);
            Assert.AreEqual(new StoneIndex(3, 5), selectStoneIndex);
        }
    }
}
