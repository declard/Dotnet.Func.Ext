namespace Dotnet.Func.Ext.Data
{
    using Algebras;
    using System;
    using System.Collections.Generic;
    using static Algebras.Structures;
    using static Ctors;
    using static Tuples;
    using static Units;

    public static partial class Ctors
    {
        /// <summary>
        /// The only Pair injection
        /// </summary>
        public static Pair<left, right> Pair<left, right>(left l, right r) => Tuples.Pair<left, right>.Create(l, r);

        /// <summary>
        /// Symmetrical Pair injection
        /// </summary>
        public static Pair<val, val> Pair<val>(val v) => Pair(v, v);

        /// <summary>
        /// KVP usual injection
        /// </summary>
        public static KeyValuePair<key, value> KeyValuePair<key, value>(key k, value v) => new KeyValuePair<key, value>(k, v);
        /// <summary>
        /// KVP inverse injection
        /// </summary>
        public static KeyValuePair<key, value> ValueKeyPair<key, value>(value v, key k) => new KeyValuePair<key, value>(k, v);
    }

    public static partial class Dtors
    {
        /// <summary>
        /// Pair left projection
        /// </summary>
        public static left Left<left, right>(this Pair<left, right> that) => that.Case(Unit(), (_, l, r) => l);
        /// <summary>
        /// Pair right projection
        /// </summary>
        public static right Right<left, right>(this Pair<left, right> that) => that.Case(Unit(), (_, l, r) => r);
    }

    /// <summary>
    /// Different tuples
    /// </summary>
    public static class Tuples
    {
        #region Pair

        /// <summary>
        /// Pair of values
        /// Is a functor by its right value
        /// </summary>
        /// <typeparam name="left">Left value</typeparam>
        /// <typeparam name="right">Right value</typeparam>
        public struct Pair<left, right>
        {
            private left _left;
            private right _right;

            /// <summary>
            /// Smart ctor
            /// </summary>
            public static Pair<left, right> Create(left Left, right Right) => new Pair<left, right>
            {
                _left = Left,
                _right = Right,
            };

            /// <summary>
            /// Basic pattern matcher (close-evading)
            /// </summary>
            public outˈ Case<ctx, outˈ>(ctx ctxˈ, Func<ctx, left, right, outˈ> f) =>
                f(ctxˈ, _left, _right);
        }

        public static class Pair
        {
            /// <summary>
            /// Pointer functor unary operation
            /// </summary>
            public static Pair<left, right> Pure<left, right>(right val, Resolver.Resolvable<SNeutral<left, Additive<Unit>>> neu = null) =>
                Pair(neu.Value().Zero(), val);
        }

        /// <summary>
        /// Basic pattern matcher
        /// </summary>
        public static outˈ Case<left, right, outˈ>(this Pair<left, right> that, Func<left, right, outˈ> f) =>
            that.Case(f, (fˈ, l, r) => fˈ(l, r));
        
        /// <summary>
        /// Bifunctor map
        /// </summary>
        public static Pair<leftOut, rightOut> BiMap<left, right, leftOut, rightOut>(Pair<left, right> that, Func<left, leftOut> f, Func<right, rightOut> g) =>
            Pair(f(that.Left()), g(that.Right()));

        /// <summary>
        /// Pair functor map
        /// </summary>
        public static Pair<left, rightOut> Map<left, right, rightOut>(this Pair<left, right> that, Func<right, rightOut> f) =>
            Pair(that.Left(), f(that.Right()));

        /// <summary>
        /// Pair monadic join (requires left to be an additive semigrop)
        /// </summary>
        public static Pair<left, right> Join<left, right>(this Pair<left, Pair<left, right>> that, Resolver.Resolvable<SSemigroup<left, Additive<Unit>>> semi = null) =>
            Pair(semi.Value().BinOp(that.Left(), that.Right().Left()), that.Right().Right());

        /// <summary>
        /// Pair mirroring (left and right get exchanged)
        /// </summary>
        public static Pair<right, left> Flip<left, right>(this Pair<left, right> pair) => Pair(pair.Right(), pair.Left());

        #endregion

        #region Tuple

        /// <summary>
        /// Tuple_2 pattern matcher
        /// </summary>
        public static outˈ Case<in1, in2, outˈ>(this Tuple<in1, in2> that, Func<in1, in2, outˈ> f) =>
            f(that.Item1, that.Item2);

        /// <summary>
        /// Tuple_3 pattern matcher
        /// </summary>
        public static outˈ Case<in1, in2, in3, outˈ>(this Tuple<in1, in2, in3> that, Func<in1, in2, in3, outˈ> f) =>
            f(that.Item1, that.Item2, that.Item3);

        /// <summary>
        /// Tuple_4 pattern matcher
        /// </summary>
        public static outˈ Case<in1, in2, in3, in4, outˈ>(this Tuple<in1, in2, in3, in4> that, Func<in1, in2, in3, in4, outˈ> f) =>
            f(that.Item1, that.Item2, that.Item3, that.Item4);

        /// <summary>
        /// Tuple_5 pattern matcher
        /// </summary>
        public static outˈ Case<in1, in2, in3, in4, in5, outˈ>(this Tuple<in1, in2, in3, in4, in5> that, Func<in1, in2, in3, in4, in5, outˈ> f) =>
            f(that.Item1, that.Item2, that.Item3, that.Item4, that.Item5);

        /// <summary>
        /// Tuple_6 pattern matcher
        /// </summary>
        public static outˈ Case<in1, in2, in3, in4, in5, in6, outˈ>(this Tuple<in1, in2, in3, in4, in5, in6> that, Func<in1, in2, in3, in4, in5, in6, outˈ> f) =>
            f(that.Item1, that.Item2, that.Item3, that.Item4, that.Item5, that.Item6);

        /// <summary>
        /// Tuple_2 functor bimap
        /// </summary>
        public static Tuple<outA, outB> BiMap<inA, inB, outA, outB>(this Tuple<inA, inB> that, Func<inA, outA> f1, Func<inB, outB> f2) =>
            Tuple.Create(f1(that.Item1), f2(that.Item2));

        /// <summary>
        /// Tuple_2 functor map
        /// </summary>
        public static Tuple<inA, outB> Map<inA, inB, outB>(this Tuple<inA, inB> that, Func<inB, outB> f) =>
            Tuple.Create(that.Item1, f(that.Item2));

        /// <summary>
        /// Tuple_2 monadic join (requires Item1 type to be an additive semigrop)
        /// </summary>
        public static Tuple<left, right> Join<left, right>(this Tuple<left, Tuple<left, right>> that, Resolver.Resolvable<SSemigroup<left, Additive<Unit>>> semi = null) =>
            that.ToPair().Map(ToPair).Join(semi).ToTuple();

        #endregion

        #region Isomorphisms

        /// <summary>
        /// KVP to Pair iso
        /// </summary>
        public static Pair<left, right> ToPair<left, right>(this KeyValuePair<left, right> that) =>
            Pair(that.Key, that.Value);

        /// <summary>
        /// Pair to KVP iso
        /// </summary>
        public static KeyValuePair<left, right> ToKvp<left, right>(this Pair<left, right> that) =>
            KeyValuePair(that.Left(), that.Right());

        /// <summary>
        /// Pair to Tuple_2 iso
        /// </summary>
        public static Tuple<left, right> ToTuple<left, right>(this Pair<left, right> pair) =>
            Tuple.Create(pair.Left(), pair.Right());

        /// <summary>
        /// Tuple_2 to Pair iso
        /// </summary>
        public static Pair<left, right> ToPair<left, right>(this Tuple<left, right> that) =>
            Pair(that.Item1, that.Item2);

        /// <summary>
        /// Tuple_3 to Pair of Pair iso
        /// </summary>
        public static Pair<a, Pair<b, c>> ToPair<a, b, c>(this Tuple<a, b, c> that) =>
            Pair(that.Item1, Pair(that.Item2, that.Item3));

        /// <summary>
        /// Tuple_4 to Pair of Pair of Pair iso
        /// </summary>
        public static Pair<a, Pair<b, Pair<c, d>>> ToPair<a, b, c, d>(this Tuple<a, b, c, d> that) =>
            Pair(that.Item1, Pair(that.Item2, Pair(that.Item3, that.Item4)));

        #endregion
    }
}
