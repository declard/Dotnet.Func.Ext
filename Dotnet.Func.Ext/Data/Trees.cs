using System;

namespace Dotnet.Func.Ext.Data
{
    using static Optionals;
    using static Orders;
    using static Tuples;
    using static Units;
    using static Algebraic.Signatures;
    using static Algebraic.Extensions;
    using Algebraic;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;
    public class Trees
    {
        public struct TreeMap<k, v> : IEnumerable<KeyValuePair<k, v>>
        {
            private WBT<Pair<k, v>>.N _tree;
            private ROrder<k, Unit> _ord;

            private static TreeMap<k, v> New(ROrder<k, Unit> ord, WBT<Pair<k, v>>.N t) => new TreeMap<k, v> { _ord = ord, _tree = t };
            
            private static WBT<Pair<k, v>>.N Alter(Func<k, Ord> p, k key, Func<Opt<v>, Opt<v>> f, WBT<Pair<k, v>>.N t) =>
               WBT<Pair<k, v>>.Tip(t)
                    ? (f(Ctors.None())).Case(_ => WBT<Pair<k, v>>.Tip(), (v vv) => WBT<Pair<k, v>>.Singleton(Ctors.Pair(key, vv)))
                    : (p(t.v.Left()).Case(
                        _ => WBT<Pair<k, v>>.Balance(t.v, Alter(p, key, f, t.l), t.r),
                        _ => f(Ctors.Some(t.v.Right())).Case(
                            __ => WBT<Pair<k, v>>.Glue(t.l, t.r),
                            vv => WBT<Pair<k, v>>.Bin(t.s, Ctors.Pair(key, vv), t.l, t.r)),
                        _ => WBT<Pair<k, v>>.Balance(t.v, t.l, Alter(p, key, f, t.r))));

            private static WBT<Pair<k, v>>.N Map(Func<k, v, v> f, WBT<Pair<k, v>>.N t) =>
                WBT<Pair<k, v>>.Tip(t) ? t : WBT<Pair<k, v>>.Bin(t.s, Ctors.Pair(t.v.Left(), t.v.Case(f)), Map(f, t.l), Map(f, t.r));

            public TreeMap<k, v> Empty(ROrder<k, Unit> ord = null) => New(ord, null);
            public TreeMap<k, v> Singleton(k key, v value, ROrder<k, Unit> ord = null) => New(ord, WBT<Pair<k, v>>.Singleton(Ctors.Pair(key, value)));
            public Opt<v> Lookup(k key) => WBT<Pair<k, v>>.Lookup(WBT<k>.MkComp(_ord, key).CoMap<Pair<k, v>, k, Ord>(Dtors.Left), _tree).Map(kv => kv.Right());
            public TreeMap<k, v> Alter(k key, Func<Opt<v>, Opt<v>> f) => New(_ord, Alter(WBT<k>.MkComp(_ord, key), key, f, _tree));
            public int Size() => WBT<Pair<k, v>>.Size(_tree);
            public TreeMap<k, v> Map(Func<k, v, v> f) => New(_ord, Map(f, _tree));

            public IEnumerator<KeyValuePair<k, v>> GetEnumerator() => _tree.Select(Tuples.ToKvp).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public struct TreeSet<v> : IEnumerable<v>
        {
            private WBT<v>.N _tree;
            private ROrder<v, Unit> _ord;

            private static TreeSet<v> New(ROrder<v, Unit> ord, WBT<v>.N t) => new TreeSet<v> { _ord = ord, _tree = t };

            public TreeSet<v> Empty(ROrder<v, Unit> ord = null) => New(ord, null);
            public TreeSet<v> Singleton(v value, ROrder<v, Unit> ord = null) => New(ord, WBT<v>.Singleton(value));
            public bool Contains(v value) => WBT<v>.Lookup(WBT<v>.MkComp(_ord, value), _tree).IsSome();
            public TreeSet<v> Insert(v value) => New(_ord, WBT<v>.Insert(WBT<v>.MkComp(_ord, value), value, _tree));
            public TreeSet<v> Delete(v value) => New(_ord, WBT<v>.Delete(WBT<v>.MkComp(_ord, value), _tree));
            public int Size() => WBT<v>.Size(_tree);

            public IEnumerator<v> GetEnumerator() => _tree.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        /// <remarks>
        /// Implementation is based on the paper "Balancing weight-balanced trees" by Yoichi Hirai and Kazuhiko Yamamoto, Cambridge University Press, 2011
        /// </remarks>
        private static class WBT<a>
        {
            public class N : IEnumerable<a>
            {
                public int s;
                public a v;
                public N l;
                public N r;

                public IEnumerator<a> GetEnumerator()
                {
                    yield return v;
                    foreach (var v in l) yield return v;
                    foreach (var v in r) yield return v;
                }

                IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            }

            public static readonly int Delta = 3;
            public static readonly int Gamma = 2;

            public static Func<a, Ord> MkComp(ROrder<a, Unit> ord, a value) =>
                v => (ord ?? AOrdComparer.Create(Ctors.Func<a, a, int>(Comparer<a>.Default.Compare).Map(Orders.ToOrd))).Compare(value, v);

            public static bool Tip(N t) => t == null;
            public static N Tip() => null;
            public static int Size(N t) => Tip(t) ? 0 : t.s;
            public static N Singleton(a v) => new N { s = 1, v = v };
            public static N Bin(a v, N l, N r) => Bin(Size(l) + Size(r) + 1, v, l, r);
            public static N Bin(int s, a v, N l, N r) => new N { s = s, v = v, l = l, r = r };

            public static bool IsBalanced(N l, N r) => Delta * (Size(l) + 1) >= Size(r) + 1;
            public static bool IsSingle(N l, N r) => Size(l) + 1 < Gamma * (Size(r) + 1);

            public static N RotateL(a v, N l, N r) => IsSingle(r.l, r.r) ? SingleL(v, l, r) : RotateL(v, l, r);
            public static N SingleL(a v, N l, N r) => Bin(r.v, Bin(v, l, r.l), r.r);
            public static N DoubleL(a v, N l, N r) => Bin(r.l.v, Bin(v, l, r.l.l), Bin(r.v, r.l.r, r.r));
            public static N BalanceL(a v, N l, N r) => IsBalanced(l, r) ? Bin(v, l, r) : RotateL(v, l, r);

            public static N RotateR(a v, N l, N r) => IsSingle(r.r, r.l) ? SingleL(v, r, l) : RotateL(v, r, l);
            public static N SingleR(a v, N l, N r) => Bin(l.v, Bin(v, r, l.r), l.l);
            public static N DoubleR(a v, N l, N r) => Bin(l.r.v, Bin(v, r, l.r.r), Bin(l.v, l.r.l, l.l));
            public static N BalanceR(a v, N l, N r) => IsBalanced(r, l) ? Bin(v, r, l) : RotateL(v, r, l);

            public static Pair<a, N> DeleteFindMin(N t)
            {
                if (Tip(t)) throw new InvalidOperationException("No minimum for an empty tree");
                if (Tip(t.l)) return Ctors.Pair(t.v, t.r);
                var min = DeleteFindMin(t.l);
                return Ctors.Pair(min.Left(), BalanceR(t.v, min.Right(), t.r));
            }

            public static Pair<a, N> DeleteFindMax(N t)
            {
                if (Tip(t)) throw new InvalidOperationException("No maximum for an empty tree");
                if (Tip(t.r)) return Ctors.Pair(t.v, t.l);
                var max = DeleteFindMax(t.r);
                return Ctors.Pair(max.Left(), BalanceL(t.v, t.l, max.Right()));
            }

            public static N Glue(N l, N r)
            {
                if (Tip(l)) return r;
                if (Tip(r)) return l;

                if (Size(l) > Size(r))
                {
                    var max = DeleteFindMax(l);
                    return BalanceR(max.Left(), max.Right(), r);
                }

                var min = DeleteFindMin(r);
                return BalanceL(min.Left(), l, min.Right());
            }

            public static N Balance(a v, N l, N r)
            {
                var sizel = Size(l);
                var sizer = Size(r);
                var sizex = sizel + sizer + 1;
                if (sizel + sizer <= 1) return Bin(sizex, v, l, r);
                if (sizer > Delta * sizel) return RotateL(v, l, r);
                if (sizel > Delta * sizer) return RotateR(v, l, r);
                return Bin(sizex, v, l, r);
            }

            public static N Insert(Func<a, Ord> c, a v, N t) => Tip(t) ? Singleton(v) : c(t.v).Case(_ => BalanceR(t.v, Insert(c, v, t.l), t.r), _ => Bin(t.s, v, t.l, t.r), _ => BalanceL(t.v, t.l, Insert(c, v, t.r)));
            public static N Delete(Func<a, Ord> p, N t) => Tip(t) ? t : p(t.v).Case(_ => BalanceR(t.v, Delete(p, t.l), t.r), _ => Glue(t.l, t.r), _ => BalanceL(t.v, t.l, Delete(p, t.r)));
            public static Opt<a> Lookup(Func<a, Ord> p, N t) => Tip(t) ? Ctors.None() : p(t.v).Case(_ => Lookup(p, t.l), _ => Ctors.Some(t.v), _ => Lookup(p, t.r));
        }
    }
}
