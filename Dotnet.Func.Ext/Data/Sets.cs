
namespace Dotnet.Func.Ext.Data
{
    using static Algebraic.Signatures;
    using static Trees;
    using static Units;
    using static Ctors;
    using static Core.Functions;

    public static class Sets
    {
        public struct Set<val>
        {
            private enum Color
            {
                Red,
                Black,
            }

            private struct Payload
            {
                public Color Color;
                public val Value;
            }

            private TipBinTree<Payload> _tree;
            private ROrder<val, Unit> _order;

            static Payload p(Color c, val v) => new Payload { Color = c, Value = v };
            static TipBinTree<Payload> t(TipBinTree<Payload>.Bin t, Payload p) => TipBinTree<Payload>.CreateBin(t.Left, p, t.Right);
            static TipBinTree<Payload> t() => TipBinTree<Payload>.CreateTip();

            public static Set<val> CreateEmpty(ROrder<val, Unit> order) => new Set<val> { _order = order, _tree = t() };

            static Color GetColor(TipBinTree<Payload> tree) => tree.Case(_ => Color.Black, n => n.Payload.Color);
            static TipBinTree<Payload> setBlack(TipBinTree<Payload> tree) => tree.Case(tree, Fst, Unit(), (u, n) => t(n, p(Color.Black, n.Payload.Value)));
            static TipBinTree<Payload> setRed(TipBinTree<Payload> tree) => tree.Case(tree, Fst, Unit(), (u, n) => t(n, p(Color.Red, n.Payload.Value)));
            static TipBinTree<Payload>.Zipper toZip(TipBinTree<Payload> tree) => TipBinTree<Payload>.Zipper.Create(tree);
            static TipBinTree<Payload> toTree(TipBinTree<Payload>.Zipper z) => z.Reconstruct();
            static TipBinTree<Payload>.Zipper leftmost(TipBinTree<Payload>.Zipper z) => z.MoveLeft().Case(z, Fst, Unit(), (u, zz) => leftmost(zz));
            static TipBinTree<Payload>.Zipper rightmost(TipBinTree<Payload>.Zipper z) => z.MoveRight().Case(z, Fst, Unit(), (u, zz) => rightmost(zz));
        }
    }
}
