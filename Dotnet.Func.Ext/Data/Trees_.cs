namespace Dotnet.Func.Ext.Data
{
    using System;
    using static Units;
    using static Core.Functions;
    using static Trees;
    using static Ctors;
    using static Exceptions;
    using static Algebraic.Signatures;
    using SCG = System.Collections.Generic;
    using System.Collections;
    using static Optionals;
    using Algebraic;
    using static Tuples;
    using static Lists;
    public static partial class Ctors
    {
        /// <summary>
        /// Empty tree ctor
        /// </summary>
        public static TipBinTree<val> Tip<val>() => TipBinTree<val>.CreateTip();
        /// <summary>
        /// Empty tree injection
        /// </summary>
        public static TipBinTree<val> Tip<val>(Unit _) => TipBinTree<val>.CreateTip();

        /// <summary>
        /// Non-empty tree injection
        /// </summary>
        public static TipBinTree<val> Bin<val>(TipBinTree<val> left, val payload, TipBinTree<val> right) => TipBinTree<val>.CreateBin(left, payload, right);
    }

    public static partial class Dtors
    {
        /// <summary>
        /// Empty tree projection
        /// </summary>
        public static Unit Tip<val>(this TipBinTree<val> that) =>
            that.Case(Unit(), Snd, Failing<Unit>.Tag(that, nameof(Tip)), (t, _) => t.ToValue());

        /// <summary>
        /// Non-empty tree projection
        /// </summary>
        public static TipBinTree<val>.Bin Bin<val>(this TipBinTree<val> that) =>
            that.Case(Failing<TipBinTree<val>.Bin>.Tag(that, nameof(Bin)), (t, _) => t.ToValue(), Unit(), Snd);

        /// <summary>
        /// Empty tree projection test
        /// </summary>
        public static bool IsTip<val>(this TipBinTree<val> that) => that.Case(true, Fst, false, Fst);
        /// <summary>
        /// Non-empty tree projection test
        /// </summary>
        public static bool IsBin<val>(this TipBinTree<val> that) => that.Case(false, Fst, true, Fst);
    }

    public static class Trees
    {
        /// <summary>
        /// Binary tree with data in internal nodes and empty terminal nodes
        /// 
        /// data TipBinTree val = Tip | Bin { left :: (TipBinTree val), payload :: val, right :: (TipBinTree val) }
        /// </summary>
        /// <typeparam name="val">Type of a payload</typeparam>
        public struct TipBinTree<val> : IEither<Unit, TipBinTree<val>.Bin>, SCG.IEnumerable<val>
        {
            /// <summary>
            /// Container to support references between nodes
            /// </summary>
            private class Node
            {
                public Bin Bin;
            }

            /// <summary>
            /// Node is a left branch, a value and a right branch
            /// </summary>
            public struct Bin
            {
                public TipBinTree<val> Left;
                public val Payload;
                public TipBinTree<val> Right;
            }

            /// <summary>
            /// Tree is a node or nothing
            /// </summary>
            private Node _node;

            /// <summary>
            /// Empty tree ctor
            /// </summary>
            public static TipBinTree<val> CreateTip() => new TipBinTree<val>();
            /// <summary>
            /// Non-empty tree ctor
            /// </summary>
            /// <param name="left">Left subtree</param>
            /// <param name="payload">Element value</param>
            /// <param name="right">Right subtree</param>
            public static TipBinTree<val> CreateBin(TipBinTree<val> left, val payload, TipBinTree<val> right) =>
                new TipBinTree<val> { _node = new Node { Bin = new Bin { Left = left, Payload = payload, Right = right } } };

            /// <summary>
            /// Basic pattern matcher (closure-evading)
            /// </summary>
            public res Case<tipCtx, binCtx, res>(tipCtx tipCtxˈ, Func<tipCtx, Unit, res> Tip, binCtx binCtxˈ, Func<binCtx, Bin, res> Bin) =>
                _node == null ? Tip(tipCtxˈ, Unit()) : Bin(binCtxˈ, _node.Bin);

            public SCG.IEnumerator<val> GetEnumerator()
            {
                if (_node == null)
                    yield break;

                var bin = _node.Bin;

                yield return bin.Payload;

                foreach (var e in bin.Left)
                    yield return e;

                foreach (var e in bin.Right)
                    yield return e;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public override string ToString() => _node == null ? "Tip()" : $"Bin({_node.Bin.Left}, {_node.Bin.Payload}, {_node.Bin.Right})";


            public struct Zipper
            {
                struct Point
                {
                    public bool IsRight;
                    public val Payload;
                    public TipBinTree<val> Another;
                }
                TipBinTree<val> _tree;
                List<Point> _way;
                
                private static Zipper Move(bool isRight, Zipper that, Bin tree) => new Zipper
                {
                    _tree = isRight ? tree.Right : tree.Left,
                    _way = Cons(new Point
                    {
                        IsRight = isRight,
                        Payload = tree.Payload,
                        Another = isRight ? tree.Left : tree.Right
                    }, that._way)
                };
                private static Zipper MoveBack(Zipper that, Pair<Point, List<Point>> pair)
                {
                    var p = pair.Left();

                    return new Zipper
                    {
                        _tree = p.IsRight ? CreateBin(p.Another, p.Payload, that._tree) : CreateBin(that._tree, p.Payload, p.Another),
                        _way = pair.Right()
                    };
                }

                private Opt<Zipper> Move(bool isRight) => _tree.Case(None<Zipper>(), Fst, this, (that, tree) => Some(Move(false, that, tree)));

                public static Zipper Create(TipBinTree<val> tree) => new Zipper { _tree = tree };
                public TipBinTree<val> Reconstruct() => Reconstruct(this).Get();
                private static Zipper Reconstruct(Zipper that) => that.MoveBack().Case(that, Fst, Unit(), (_, z) => Reconstruct(z));

                public TipBinTree<val> Get() => _tree;
                public Zipper Set(TipBinTree<val> tree) => new Zipper { _tree = tree, _way = _way };

                public Opt<Zipper> MoveLeft() => Move(false);
                public Opt<Zipper> MoveRight() => Move(true);
                public Opt<Zipper> MoveBack() => _way.Case(None<Zipper>(), Fst, this, (that, p) => Some(MoveBack(that, p)));

                public res Case<res>(Func<val, TipBinTree<val>, Zipper, res> Left, Func<Unit, res> Empty, Func<val, TipBinTree<val>, Zipper, res> Right) =>
                    _way.Case(Nil, App, this, (that, p) => (p.Left().IsRight ? Right : Left)(p.Left().Payload, p.Left().Another, that.MoveBack().Some()));
            }
        }
        
        public static class TipBinTree
        {
            public static TipBinTree<val> Pure<val>(val value) => TipBinTree<val>.CreateBin(TipBinTree<val>.CreateTip(), value, TipBinTree<val>.CreateTip());
        }

        public static res Case<val, res>(this TipBinTree<val> that, Func<Unit, res> tip, Func<TipBinTree<val>.Bin, res> bin) =>
            that.Case(tip, App, bin, App);

        public static Opt<TipBinTree<val>.Bin> TryBin<val>(this TipBinTree<val> that) => that.Homo(ATipBinTree<val>.Class, AOpt<TipBinTree<val>.Bin>.Class);
    }
}
