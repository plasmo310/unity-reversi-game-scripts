using System;
using NUnit.Framework;
using Reversi.Stones.Stone;

namespace Reversi.Tests
{
    /// <summary>
    /// ストーン関連のテスト共通処理
    /// </summary>
    public static class StoneTestUtil
    {
        /// <summary>
        /// int配列からStoneStates配列に変換
        /// </summary>
        /// <param name="stoneNumbers"></param>
        /// <returns></returns>
        public static StoneState[,] ConvertNumbersToStoneStates(int[,] stoneNumbers)
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
        
        /// <summary>
        /// StoneStates同士の比較
        /// </summary>
        /// <param name="stoneStatesA"></param>
        /// <param name="stoneStatesB"></param>
        public static void AssertStoneStates(StoneState[,] stoneStatesA, StoneState[,] stoneStatesB)
        {
            Assert.AreEqual(stoneStatesA.GetLength(0), stoneStatesB.GetLength(0));
            Assert.AreEqual(stoneStatesA.GetLength(1), stoneStatesB.GetLength(1));
            for (var x = 0; x < stoneStatesA.GetLength(0); x++)
            {
                for (var z = 0; z < stoneStatesA.GetLength(1); z++)
                {
                    Assert.AreEqual(true, stoneStatesA[x, z] == stoneStatesB[x, z]);
                }
            }
        }
    }
}
