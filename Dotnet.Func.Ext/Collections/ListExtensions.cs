
namespace Dotnet.Func.Ext.Collections
{
    using System;
    using System.Collections.Generic;

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
    }
}
