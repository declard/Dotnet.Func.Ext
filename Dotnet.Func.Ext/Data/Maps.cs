namespace Dotnet.Func.Ext.Data
{
    using static Optionals;
    using static Trees;
    using static Ctors;
    using static Units;
    using Algebraic;
    using static Algebraic.Signatures;
    using System;
    using SCG = System.Collections.Generic;
    using static Tuples;

    public static class Maps
    {
        public struct Map<key, value>
        {
            private struct Payload
            {
                public bool IsRed;
                public key Key;
                public value Value;
            }

            private TipBinTree<Payload> _tree;
            private ROrder<key, Unit> _order;
            
            public static Map<key, value> CreateEmpty(ROrder<key, Unit> order) => new Map<key, value> { _order = order };
            public static Map<key, value> CreateSingle(ROrder<key, Unit> order, key k, value v) => new Map<key, value> { _order = order, _tree = Walker.Single(k, v) };

            //public Map<key, value> Adjust(key k, Func<Opt<value>, Opt<value>> f) => new Adjuster { Order = Order, f = f, k = k }.Adjust(Tree);

            //private class Adjuster : Walker
            //{
                //public key k;
                //public Func<Opt<value>, Opt<value>> f;

                //public Map<key, value> Adjust(TipBinTree<Payload> tree) => new Map<key, value> { Order = Order, Tree = Walk(tree) };

                //private TipBinTree<Payload> Walk(TipBinTree<Payload> tree) => tree.Case(
                //    _ => f(None<value>()).Case(__ => tree, v => Single(k, v)),
                //    n => Order.Compare(k, n.Payload.Key).Case(
                //        _ => Balance(Walk(n.Left), n.Payload, n.Right),
                //        _ => f(Some(n.Payload.Value)).Case(
                //            __ => Glue(n.Left, n.Right),
                //            v => Bin(k, v, n.Left, n.Right)),
                //        _ => Balance(n.Left, n.Payload, Walk(n.Right))));
            //}

            private class Walker
            {
                public static Payload Load(key k, value v, int s) => new Payload { Key = k, Value = v, Size = s };
                public static TipBinTree<Payload> Single(key k, value v) => TipBinTree<Payload>.CreateBin(TipBinTree<Payload>.CreateTip(), Load(k, v, 1), TipBinTree<Payload>.CreateTip());
                public static TipBinTree<Payload> Bin(key k, value v, TipBinTree<Payload> l, TipBinTree<Payload> r) =>
                    TipBinTree<Payload>.CreateBin(l, Load(k, v, 1 + Size(l) + Size(r)), r);
                public static int Size(TipBinTree<Payload> t) => t.Case(_ => 0, n => n.Payload.Size);
                public static TipBinTree<Payload> Empty => TipBinTree<Payload>.CreateTip();

                public ROrder<key, Unit> Order;

                //public TipBinTree<Payload> Balance(TipBinTree<Payload> l, Payload p, TipBinTree<Payload> r) =>;
                //public TipBinTree<Payload> Glue(TipBinTree<Payload> l, TipBinTree<Payload> r) =>;
            }
            
            public Map<key, value> Adjust(key k, Func<Opt<value>, Opt<value>> f)
            {
                var z = TipBinTree<Payload>.Zipper.Create(_tree);

                while (true)
                {
                    var maybeT = z.Get().TryBin();

                    if (maybeT.IsNone())
                    {
                        var r = f(None<value>());
                        if (r.IsNone()) return this;
                            return Rebuild(_order, k, r.Some(), z);
                    }

                    var t = maybeT.Some();

                    var c = _order.Compare(k, t.Payload.Key);

                    if (c.IsEq())
                        return Some(t.Payload.Value);

                    if (c.IsLt())
                        tree = t.Left;

                    if (c.IsGt())
                        tree = t.Right;
                }
            }

            private Map<key, value> Rebuild(ROrder<key, Unit> order, key k, value v, TipBinTree<Payload>.Zipper z) =>
                z.MoveBack().Case(Unit(), (u, _) => CreateSingle(order, k, v), Unit(), (u, zz) => zz.Set(zz.Get()).Reconstruct());

            public Opt<value> Lookup(key k)
            {
                TipBinTree<Payload> tree = _tree;

                while(true)
                {
                    var maybeT = tree.TryBin();

                    if (maybeT.IsNone())
                        return None<value>();

                    var t = maybeT.Some();

                    var c = _order.Compare(k, t.Payload.Key);

                    if (c.IsEq())
                        return Some(t.Payload.Value);

                    if (c.IsLt())
                        tree = t.Left;

                    if (c.IsGt())
                        tree = t.Right;
                }
            }
        }
    }
}
