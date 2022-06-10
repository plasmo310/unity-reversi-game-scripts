using System;
using UnityEngine;

namespace Reversi.Stones.Stone
{
    /// <summary>
    /// ストーン配列に対応するインデックス
    /// </summary>
    public class StoneIndex : IEquatable<StoneIndex>
    {
        public int X { get; private set; }
        public int Z { get; private set; }
        public StoneIndex(int x, int z)
        {
            X = x;
            Z = z;
        }

        public float GetLength()
        {
            return Mathf.Sqrt(X * X + Z * Z);
        }

        public static StoneIndex operator+ (StoneIndex a, StoneIndex b)
        {
            return new StoneIndex(a.X + b.X, a.Z + b.Z);
        }

        public static StoneIndex operator- (StoneIndex a, StoneIndex b)
        {
            return new StoneIndex(a.X - b.X, a.Z - b.Z);
        }

        public bool Equals(StoneIndex other)
        {
            if (other == null)
            {
                return false;
            }
            return X == other.X && Z == other.Z;
        }
    }
}
