using NUnit.Framework;
using Reversi.Stones;
using Reversi.Stones.Stone;

namespace Reversi.Tests
{
    /// <summary>
    /// ストーン計算処理のテスト
    /// </summary>
    public class StoneCalculatorTest
    {
        [Test]
        public void T01_ストーンの色が正しくカウントされている()
        {
            // 白: 2 黒:2 の場合
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
            StoneCalculator.CheckStoneColorCount(stoneStates, (whiteCount, blackCount) =>
            {
                Assert.AreEqual(2, whiteCount);
                Assert.AreEqual(2, blackCount);
            });
            
            // 白: 2 黒:5 の場合
            stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 0, 0},
                { 0, 0, 2, 2, 1, 0, 0, 0},
                { 0, 0, 0, 2, 2, 0, 0, 0},
                { 0, 0, 0, 0, 0, 2, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            StoneCalculator.CheckStoneColorCount(stoneStates, (whiteCount, blackCount) =>
            {
                Assert.AreEqual(2, whiteCount);
                Assert.AreEqual(5, blackCount);
            });
        }
        
        [Test]
        public void T02_ひっくり返せるストーン配列が取得できる()
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
            // 白を置いた場合
            // 一枚もひっくり返せない場合
            var turnStonesIndex =  StoneCalculator.GetTurnStonesIndex(stoneStates, StoneState.White, 3, 4);
            Assert.AreEqual(0, turnStonesIndex.Count);
            // 一枚ひっくり返せる場合
            turnStonesIndex =  StoneCalculator.GetTurnStonesIndex(stoneStates, StoneState.White, 2, 4);
            Assert.AreEqual(1, turnStonesIndex.Count);
            Assert.AreEqual(new StoneIndex(3, 4), turnStonesIndex[0]);
            
            // 黒を置いた場合
            // 一枚もひっくり返せない場合
            turnStonesIndex =  StoneCalculator.GetTurnStonesIndex(stoneStates, StoneState.Black, 0, 0);
            Assert.AreEqual(0, turnStonesIndex.Count);
            // 一枚ひっくり返せる場合
            turnStonesIndex =  StoneCalculator.GetTurnStonesIndex(stoneStates, StoneState.Black, 2, 3);
            Assert.AreEqual(1, turnStonesIndex.Count);
            Assert.AreEqual(new StoneIndex(3, 3), turnStonesIndex[0]);
            
            // 八方向全てにひっくり返せるストーンがある場合
            stoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 2, 0, 0, 2, 0, 0, 2, 0},
                { 0, 1, 0, 1, 0, 1, 0, 0},
                { 0, 0, 1, 1, 1, 0, 0, 0},
                { 2, 1, 1, 0, 1, 1, 2, 0},
                { 0, 0, 1, 1, 1, 0, 0, 0},
                { 0, 1, 0, 1, 0, 1, 0, 0},
                { 2, 0, 0, 2, 0, 0, 2, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            turnStonesIndex =  StoneCalculator.GetTurnStonesIndex(stoneStates, StoneState.Black, 3, 3);
            Assert.AreEqual(16, turnStonesIndex.Count); // 2*8方向
        }
        
        [Test]
        public void T03_置くことが可能なストーン配列が取得できる()
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
            
            // 白を置いた場合
            var canPutStonesIndex = StoneCalculator.GetAllCanPutStonesIndex(stoneStates, StoneState.White);
            Assert.AreEqual(4, canPutStonesIndex.Count);
            Assert.AreEqual(new StoneIndex(2, 4), canPutStonesIndex[0]);
            Assert.AreEqual(new StoneIndex(3, 5), canPutStonesIndex[1]);
            Assert.AreEqual(new StoneIndex(4, 2), canPutStonesIndex[2]);
            Assert.AreEqual(new StoneIndex(5, 3), canPutStonesIndex[3]);
            
            // 黒を置いた場合
            canPutStonesIndex = StoneCalculator.GetAllCanPutStonesIndex(stoneStates, StoneState.Black);
            Assert.AreEqual(4, canPutStonesIndex.Count);
            Assert.AreEqual(new StoneIndex(2, 3), canPutStonesIndex[0]);
            Assert.AreEqual(new StoneIndex(3, 2), canPutStonesIndex[1]);
            Assert.AreEqual(new StoneIndex(4, 5), canPutStonesIndex[2]);
            Assert.AreEqual(new StoneIndex(5, 4), canPutStonesIndex[3]);
        }
        
        [Test]
        public void T04_置いた後のストーン配列が取得できる()
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
            
            // 白を置いた場合
            var putStoneState = StoneCalculator.GetPutStoneState(stoneStates, StoneState.White, 2, 4);
            var expectStoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 1, 0, 0, 0},
                { 0, 0, 0, 1, 1, 0, 0, 0},
                { 0, 0, 0, 2, 1, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            StoneTestUtil.AssertStoneStates(putStoneState, expectStoneStates);
            
            // 黒を置いた場合
            putStoneState = StoneCalculator.GetPutStoneState(stoneStates, StoneState.Black, 4, 5);
            expectStoneStates = StoneTestUtil.ConvertNumbersToStoneStates(new[,] {
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 1, 2, 0, 0, 0},
                { 0, 0, 0, 2, 2, 2, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
                { 0, 0, 0, 0, 0, 0, 0, 0},
            });
            StoneTestUtil.AssertStoneStates(putStoneState, expectStoneStates);
        }
    }
}
