
namespace Dotnet.Func.Ext.Collections
{
    using Dotnet.Func.Ext.Algebraic;
    using Dotnet.Func.Ext.Collections.Lazy;
    using Dotnet.Func.Ext.Data;
    using System;
    using System.Collections.Generic;
    using static Dotnet.Func.Ext.Data.Optionals;
    using static Dotnet.Func.Ext.Data.Orders;

    public static class ListExtensions
    {
        /// <summary>
        /// Sugar to allow code like `new List{Tuple{t1, t2}} { { v1, v2 } }`
        /// </summary>
        public static void Add<in1, in2>(this List<Tuple<in1, in2>> that, in1 in1ˈ, in2 in2ˈ) =>
            that.Add(Tuple.Create(in1ˈ, in2ˈ));

        /// <summary>
        /// Sugar to allow code like `new List{Tuple{t1, t2, t3}} { { v1, v2, v3 } }`
        /// </summary>
        public static void Add<in1, in2, in3>(this List<Tuple<in1, in2, in3>> that, in1 in1ˈ, in2 in2ˈ, in3 in3ˈ) =>
            that.Add(Tuple.Create(in1ˈ, in2ˈ, in3ˈ));

        /// <summary>
        /// Sugar to allow code like `new List{Tuple{t1, t2, t3, t4}} { { v1, v2, v3, v4 } }`
        /// </summary>
        public static void Add<in1, in2, in3, in4>(this List<Tuple<in1, in2, in3, in4>> that, in1 in1ˈ, in2 in2ˈ, in3 in3ˈ, in4 in4ˈ) =>
            that.Add(Tuple.Create(in1ˈ, in2ˈ, in3ˈ, in4ˈ));

        /// <summary>
        /// Sugar to allow code like `new List{Tuple{t1, t2, t3, t4, t5}} { { v1, v2, v3, v4, v5 } }`
        /// </summary>
        public static void Add<in1, in2, in3, in4, in5>(this List<Tuple<in1, in2, in3, in4, in5>> that, in1 in1ˈ, in2 in2ˈ, in3 in3ˈ, in4 in4ˈ, in5 in5ˈ) =>
            that.Add(Tuple.Create(in1ˈ, in2ˈ, in3ˈ, in4ˈ, in5ˈ));

        /// <summary>
        /// Sugar to allow code like `new List{Tuple{t1, t2, t3, t4, t5, t6}} { { v1, v2, v3, v4, v5, v6 } }`
        /// </summary>
        public static void Add<in1, in2, in3, in4, in5, in6>(this List<Tuple<in1, in2, in3, in4, in5, in6>> that, in1 in1ˈ, in2 in2ˈ, in3 in3ˈ, in4 in4ˈ, in5 in5ˈ, in6 in6ˈ) =>
            that.Add(Tuple.Create(in1ˈ, in2ˈ, in3ˈ, in4ˈ, in5ˈ, in6ˈ));

        public static IReadOnlyList<element> Slice<element>(this IReadOnlyList<element> that, int offset, int length) =>
            new ReadOnlyListSlice<element>(that, offset, length);

        public static Opt<int> BinarySearch<element>(this IReadOnlyList<element> that, int index, int length, element value, IComparer<element> comparer = null, Ord fallback = default(Ord)) =>
            that.Slice(index, length).BinarySearch(value, comparer, fallback).Map(index, (i, r) => r + i);

        public static Opt<int> BinarySearch<element>(this IReadOnlyList<element> list, element value, IComparer<element> comparer = null, Ord fallback = default(Ord))
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            int lo = 0;
            int hi = list.Count - 1;
            comparer = comparer ?? Comparer<element>.Default;

            while (lo <= hi)
            {
                int i = lo + ((hi - lo) >> 1);

                int c = comparer.Compare(list[i], value);
                if (c == 0) return i;
                if (c < 0)
                {
                    lo = i + 1;
                }
                else
                {
                    hi = i - 1;
                }
            }

            var fallenBack = fallback.Case(lo - 1, -1, lo);

            return fallenBack.IsOnSemiInterval(0, list.Count, AInt32.Class).ThenOrDefault(fallenBack.PureOpt());
        }
    }
}
