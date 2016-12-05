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

    public class Trees
    {
        public struct Map<k, v>
        {
            private WBT<Pair<k, v>>.N _tree;
            private ROrder<k, Unit> _ord;

            private static Map<k, v> New(ROrder<k, Unit> ord, WBT<Pair<k, v>>.N t) => new Map<k, v> { _ord = ord, _tree = t };
            
            private static WBT<Pair<k, v>>.N Alter(Func<k, Ord> p, k key, Func<Opt<v>, Opt<v>> f, WBT<Pair<k, v>>.N t) =>
               WBT<Pair<k, v>>.Tip(t)
                    ? (f(Ctors.None<v>())).Case(_ => null, (v vv) => WBT<Pair<k, v>>.Singleton(Ctors.Pair(key, vv)))
                    : (p(t.v.Left()).Case(
                        _ => WBT<Pair<k, v>>.Balance(t.v, Alter(p, key, f, t.l), t.r),
                        _ => f(Ctors.Some(t.v.Right())).Case(
                            __ => WBT<Pair<k, v>>.Glue(t.l, t.r),
                            vv => WBT<Pair<k, v>>.Bin(t.s, Ctors.Pair(key, vv), t.l, t.r)),
                        _ => WBT<Pair<k, v>>.Balance(t.v, t.l, Alter(p, key, f, t.r))));

            private static Func<k, Ord> MkComp(ROrder<k, Unit> ord, k key) =>
                kk => (ord ?? AOrdComparer.Create(Ctors.Func<k, k, int>(Comparer<k>.Default.Compare).Map(Orders.ToOrd))).Compare(key, kk);

            public Map<k, v> Empty(ROrder<k, Unit> ord = null) => New(ord, null);
            public Map<k, v> Singleton(k key, v value, ROrder<k, Unit> ord = null) => New(ord, WBT<Pair<k, v>>.Singleton(Ctors.Pair(key, value)));
            public Opt<v> Lookup(k key) => WBT<Pair<k, v>>.Lookup(MkComp(_ord, key).CoMap<Pair<k, v>, k, Ord>(Dtors.Left), _tree).Map(kv => kv.Right());
            public Map<k, v> Alter(k key, Func<Opt<v>, Opt<v>> f) => New(_ord, Alter(MkComp(_ord, key), key, f, _tree));
            public int Size() => WBT<Pair<k, v>>.Size(_tree);
        }

        public struct Set<v>
        {
            private WBT<v>.N _tree;
            private ROrder<v, Unit> _ord;
            private ROrder<v, Unit> Ord => _ord ?? (_ord = AOrdComparer.Create(Ctors.Func<v, v, int>(Comparer<v>.Default.Compare).Map(Orders.ToOrd)));

            private static Set<v> New(ROrder<v, Unit> ord, WBT<v>.N t) => new Set<v> { _ord = ord, _tree = t };

            private static Func<v, Ord> MkComp(ROrder<v, Unit> ord, v value) =>
                vv => (ord ?? AOrdComparer.Create(Ctors.Func<v, v, int>(Comparer<v>.Default.Compare).Map(Orders.ToOrd))).Compare(value, vv);

            public Set<v> Empty(ROrder<v, Unit> ord = null) => New(ord, null);
            public Set<v> Singleton(v value, ROrder<v, Unit> ord = null) => New(ord, WBT<v>.Singleton(value));
            public bool Contains(v value) => WBT<v>.Lookup(MkComp(_ord, value), _tree).IsSome();
            public Set<v> Insert(v value) => New(_ord, WBT<v>.Insert(MkComp(_ord, value), value, _tree));
            public Set<v> Delete(v value) => New(_ord, WBT<v>.Delete(MkComp(_ord, value), _tree));
            public int Size() => WBT<v>.Size(_tree);
        }

        /// <remarks>
        /// Implementation is based on the paper "Balancing weight-balanced trees" by Yoichi Hirai and Kazuhiko Yamamoto, Cambridge University Press, 2011
        /// </remarks>
        private static class WBT<a>
        {
            public class N
            {
                public int s;
                public a v;
                public N l;
                public N r;
            }

            public static readonly int Delta = 3;
            public static readonly int Gamma = 2;

            public static bool Tip(N t) => t == null;
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
            public static Opt<a> Lookup(Func<a, Ord> p, N t) => Tip(t) ? Ctors.None<a>() : p(t.v).Case(_ => Lookup(p, t.l), _ => Ctors.Some(t.v), _ => Lookup(p, t.r));
        }
    }
}
