using System;
using System.Collections.Generic;

namespace CSG.Sharp.Extensions
{
    internal static class EnumerableExtensions
    {
        public static TItem[] Slice<TItem>(this IList<TItem> source, int start = 0, int end = 0)
        {
            /*if (end == 0) end = source.Count;

            if (end < 0)
            {
                end = source.Count + end;
            }

            int len = end - start;

            TItem[] res = new TItem[len];

            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }*/

            return new List<TItem>(source).ToArray();
        }

        public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> doActionFor)
        {
            foreach (TItem item in items)
            {
                doActionFor(item);
            }
        }
    }
}