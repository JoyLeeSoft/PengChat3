using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PC3API_dn
{
    internal static class Utility
    {
        public static T[] CombineArray<T>(params T[][] arrays)
        {
            int length = 0;
            foreach (T[] array in arrays)
                length += array.Length;

            T[] rv = new T[length];
            int offset = 0;

            foreach (T[] array in arrays)
            {
                Buffer.BlockCopy(array, 0, rv, offset, array.Length);
                offset += array.Length;
            }

            return rv;
        }

        public static T[][] SplitArray<T>(T[] arr, T keyword)
        {
            var result = new List<List<T>>();

            var piece = new List<T>();

            foreach (T b in arr)
            {
                if (b.Equals(keyword))
                {
                    result.Add(piece);
                    piece = new List<T>();
                }
                else
                {
                    piece.Add(b);
                }
            }

            if (piece.Count != 0)
                result.Add(piece);

            var res = new T[result.Count][];

            for (int i = 0; i < result.Count; i++)
            {
                res[i] = result[i].ToArray();
            }

            return res;
        }
    }
}