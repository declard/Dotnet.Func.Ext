using System;

namespace Dotnet.Func.Ext.Data
{
    using System.Collections;
    using static Algebraic.Signatures;
    using static Ctors;
    using static Exceptions;
    using static Lists;
    using static Optionals;
    using static Tuples;
    using static Units;
    using static Core.Functions;
    using Algebraic;
    using SCG = System.Collections.Generic;

    public static partial class Ctors
    {
        /// <summary>
        /// Empty list ctor
        /// </summary>
        public static List<val> Nil<val>() => List<val>.CreateNil();
        /// <summary>
        /// Empty list injection
        /// </summary>
        public static List<val> Nil<val>(Unit _) => List<val>.CreateNil();

        /// <summary>
        /// Non-empty list injection
        /// </summary>
        public static List<val> Cons<val>(val head, List<val> tail) => List<val>.CreateCons(head, tail);
    }

    public static partial class Dtors
    {
        /// <summary>
        /// Empty list projection
        /// </summary>
        public static Unit Nil<val>(this List<val> that) =>
            that.Case(Unit(), Snd, Failing<Unit>.Tag(that, nameof(Nil)), (t, _) => t.ToValue());

        /// <summary>
        /// Non-empty list projection
        /// </summary>
        public static Pair<val, List<val>> Cons<val>(this List<val> that) =>
            that.Case(Failing<Pair<val, List<val>>>.Tag(that, nameof(Cons)), (t, _) => t.ToValue(), Unit(), Snd);

        /// <summary>
        /// Empty list projection test
        /// </summary>
        public static bool IsNil<val>(this List<val> that) => that.Case(true, Fst, false, Fst);
        /// <summary>
        /// Non-empty list projection test
        /// </summary>
        public static bool IsCons<val>(this List<val> that) => that.Case(false, Fst, true, Fst);
    }

    public static class Lists
    {
        /// <summary>
        /// Immutable linked list
        /// Here and after compact notation is used: `Cons(1, Cons(2, Cons(3, Nil())))` is denoted as `[1,2,3]` or `1:2:3:[]`
        /// 
        /// data List val = Nil | Cons { head :: val, tail :: (List val) }
        /// </summary>
        /// <typeparam name="val">Element type</typeparam>
        public struct List<val> : IEither<Unit, Pair<val, List<val>>>, SCG.IEnumerable<val>
        {
            /// <summary>
            /// Container to support references between nodes
            /// </summary>
            private class Node
            {
                public Cons Cons;
            }

            /// <summary>
            /// Node is a value and a list
            /// </summary>
            public struct Cons
            {
                public val Head;
                public List<val> Tail;
            }

            /// <summary>
            /// List is a node or nothing
            /// </summary>
            private Node _node;
            
            /// <summary>
            /// Empty list ctor
            /// </summary>
            public static List<val> CreateNil() => new List<val>();

            /// <summary>
            /// Non-empty list ctor
            /// </summary>
            /// <param name="head">Element value</param>
            /// <param name="tail">List's tail</param>
            public static List<val> CreateCons(val head, List<val> tail) =>
                new List<val> { _node = new Node { Cons = new Cons { Head = head, Tail = tail } } };

            /// <summary>
            /// Basic pattern matcher (closure-evading)
            /// </summary>
            public res Case<leftCtx, rightCtx, res>(leftCtx leftCtxˈ, Func<leftCtx, Unit, res> Left, rightCtx rightCtxˈ, Func<rightCtx, Pair<val, List<val>>, res> Right) =>
                _node == null ? Left(leftCtxˈ, Unit()) : Right(rightCtxˈ, Pair(_node.Cons.Head, _node.Cons.Tail));

            public SCG.IEnumerator<val> GetEnumerator()
            {
                var next = this;
                while (next.IsCons())
                {
                    yield return next.Head();
                    next = next.Tail();
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public override string ToString() => _node == null ? "Nil()" : $"Cons({_node.Cons.Head}, {_node.Cons.Tail})";
        }

        public static class List
        {
            /// <summary>
            /// Pointed functor unary operation
            /// </summary>
            /// <example>pure 1 → [1]</example>
            public static List<val> Pure<val>(val e) => Ctors.Cons(e, Nil<val>());
        }

        /// <summary>
        /// Point operation as an extension
        /// </summary>
        public static List<val> PureList<val>(this val that) => List.Pure(that);

        /// <summary>
        /// Extend a list with an element
        /// </summary>
        /// <example>cons [1,2,3] 4 → [1,2,3,4]</example>
        public static List<val> Cons<val>(this List<val> that, val value) => Ctors.Cons(value, that);

        /// <summary>
        /// Take list head or fail
        /// </summary>
        /// <example>
        /// head [1] → 1
        /// head [] → ⊥
        /// </example>
        public static val Head<val>(this List<val> that) => that.Case(InvalidOperation<val>, Dtors.Left);

        /// <summary>
        /// Take list tail or fail
        /// </summary>
        /// <example>
        /// tail [1] → []
        /// tail [] → ⊥
        /// </example>
        public static List<val> Tail<val>(this List<val> that) => that.Case(InvalidOperation<List<val>>, Dtors.Right);

        /// <summary>
        /// Try take cons
        /// </summary>
        public static Opt<Pair<val, List<val>>> TryCons<val>(this List<val> that) => that.Homo(AList<val>.Class, AOpt<Pair<val, List<val>>>.Class);

        /// <summary>
        /// Right fold (`f v (f v (f v acc))`)
        /// </summary>
        /// <example>foldr [1,2,3] [] (λv acc.v : acc) → [1,2,3]</example>
        /// <see cref="https://en.wikipedia.org/wiki/Fold_(higher-order_function)"/>
        public static outˈ Foldr<val, outˈ>(this List<val> that, outˈ acc, Func<val, outˈ, outˈ> f) =>
            that.Case(acc, Fst, Pair(acc, f), (ctx, pair) => ctx.Right()(pair.Left(), pair.Right().Foldr(ctx.Left(), ctx.Right())));

        /// <summary>
        /// Left fold (`f (f (f acc v) v) v`)
        /// </summary>
        /// <example>foldl [1,2,3] [] (λacc v.v : acc) → [3,2,1]</example>
        /// <see cref="https://en.wikipedia.org/wiki/Fold_(higher-order_function)"/>
        public static outˈ Foldl<val, outˈ>(this List<val> that, outˈ acc, Func<outˈ, val, outˈ> f) =>
            that.Case(acc, Fst, Pair(acc, f), (ctx, pair) => pair.Right().Foldl(ctx.Right()(ctx.Left(), pair.Left()), ctx.Right()));

        /// <summary>
        /// Concatenate one list with other
        /// </summary>
        /// <example>append [1,2] [3,4] → [1,2,3,4]</example>
        public static List<val> Append<val>(this List<val> that, List<val> yonder) =>
            that.Foldr(yonder, Ctors.Cons);

        /// <summary>
        /// Reverse a list
        /// </summary>
        /// <example>reverse [1,2,3] → [3,2,1]</example>
        public static List<val> Reverse<val>(this List<val> that) => that.Foldl(Nil<val>(), Cons);

        /// <summary>
        /// Functor map (by-element map)
        /// </summary>
        /// <example>map (λv.v * 2) [1,2,3] → [2,4,6]</example>
        public static List<outˈ> Map<inˈ, outˈ>(this List<inˈ> that, Func<inˈ, outˈ> f) =>
            that.Case(Unit(), (u, _) => Nil<outˈ>(u), f, (fˈ, pair) => pair.Right().Map(fˈ).Cons(fˈ(pair.Left())));

        /// <summary>
        /// Monadic join (list flattening)
        /// </summary>
        /// <example>join [[1,2],[3,4,5]] → [1,2,3,4,5]</example>
        public static List<val> Flatten<val>(this List<List<val>> that) =>
            that.Foldr(Nil<val>(), Append);

        /// <summary>
        /// Filter
        /// </summary>
        /// <example>filter [1,2,3,4] (λv.isEven v) → [2,4]</example>
        public static List<val> Filter<val>(this List<val> that, Func<val, bool> pred) =>
            that.Foldr(Nil<val>(), (e, acc) => pred(e) ? acc.Cons(e) : acc);
    }
}
