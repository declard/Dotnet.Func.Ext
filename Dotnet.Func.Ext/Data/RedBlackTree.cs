using Dotnet.Func.Ext.Algebraic;
using Dotnet.Func.Ext.Core;
using static Dotnet.Func.Ext.Algebraic.Signatures;
using static Dotnet.Func.Ext.Data.Units;

namespace Dotnet.Func.Ext.Data
{
    public class RedBlackTree
    {
        public enum Color
        {
            Red,
            Black,
        }

        public class RBTree<a>
        {
            public Color Color;
            public a Value;
            public RBTree<a> Left;
            public RBTree<a> Right;
        }

        public enum Direction
        {
            ToLeft,
            ToRight,
        }

        public struct Step<a>
        {
            public Color Color;
            public a Value;
            public Direction Direction;
            public RBTree<a> Tree;
        }

        public class Path<a>
        {
            public Step<a> Step;
            public Path<a> Next;
        }

        public struct RBZip<a>
        {
            public RBTree<a> Tree;
            public Path<a> Path;
        }

        public static RBTree<a> N<a>(Color c, a v, RBTree<a> l, RBTree<a> r) => new RBTree<a> { Color = c, Left = l, Right = r, Value = v };
        public static RBZip<a> Z<a>(RBTree<a> t, Path<a> p) => new RBZip<a> { Path = p, Tree = t };
        public static Path<a> P<a>(Step<a> s, Path<a> p) => new Path<a> { Next = p, Step = s };
        public static Step<a> S<a>(Color c, a v, Direction d, RBTree<a> t) => new Step<a> { Direction = d, Color = c, Tree = t, Value = v };

        public static RBTree<a> emptyRB<a>() => null;
        public static Color getColor<a>(RBTree<a> t) => t?.Color ?? Color.Black;
        public static RBTree<a> setBlack<a>(RBTree<a> t) => t?.FeedTo(tt => N(Color.Black, tt.Value, tt.Left, tt.Right));
        public static RBTree<a> setRed<a>(RBTree<a> t) => t?.FeedTo(tt => N(Color.Red, tt.Value, tt.Left, tt.Right));
        public static RBZip<a> toZip<a>(RBTree<a> t) => new RBZip<a> { Tree = t };
        public static RBTree<a> toTree<a>(RBZip<a> z) => topMostZip(z).Tree;

        public static RBZip<a> topMostZip<a>(RBZip<a> z) => z.Path == null ? z : (z.Path.Step.Direction == Direction.ToLeft
            ? topMostZip(Z(N(z.Path.Step.Color, z.Path.Step.Value, z.Tree, z.Path.Step.Tree), z.Path.Next))
            : topMostZip(Z(N(z.Path.Step.Color, z.Path.Step.Value, z.Path.Step.Tree, z.Tree), z.Path.Next)));

        public static RBZip<a> leftMostZip<a>(RBZip<a> z) => z.Tree == null || z.Tree.Left == null ? z : leftMostZip(Z(z.Tree.Left, P(S(z.Tree.Color, z.Tree.Value, Direction.ToLeft, z.Tree.Right), z.Path)));
        public static RBZip<a> rightMostZip<a>(RBZip<a> z) => z.Tree == null || z.Tree.Right == null ? z : leftMostZip(Z(z.Tree.Right, P(S(z.Tree.Color, z.Tree.Value, Direction.ToRight, z.Tree.Left), z.Path)));

        public static RBZip<a> leftParentZip<a>(RBZip<a> z) => z.Path == null ? Z<a>(null, null) : (z.Path.Step.Direction == Direction.ToLeft
            ? leftParentZip(Z(N(z.Path.Step.Color, z.Path.Step.Value, z.Tree, z.Path.Step.Tree), z.Path.Next))
            : Z(N(z.Path.Step.Color, z.Path.Step.Value, z.Path.Step.Tree, z.Tree), z.Path.Next));

        public static RBZip<a> rightParentZip<a>(RBZip<a> z) => z.Path == null ? Z<a>(null, null) : (z.Path.Step.Direction == Direction.ToRight
            ? rightParentZip(Z(N(z.Path.Step.Color, z.Path.Step.Value, z.Path.Step.Tree, z.Tree), z.Path.Next))
            : Z(N(z.Path.Step.Color, z.Path.Step.Value, z.Tree, z.Path.Step.Tree), z.Path.Next));

        public static RBZip<a> predZip<a>(RBZip<a> z)
        {
            if (z.Tree?.Left != null)
                return rightMostZip(Z(z.Tree.Left, P(S(z.Tree.Color, z.Tree.Value, Direction.ToLeft, z.Tree.Right), z.Path)));

            var lp = leftParentZip(z);
            if (lp.Tree == null && lp.Path == null)
                return z.Tree == null ? z : Z(z.Tree.Left, P(S(z.Tree.Color, z.Tree.Value, Direction.ToLeft, z.Tree.Right), z.Path));

                return lp;
        }

        public static RBZip<a> succZip<a>(RBZip<a> z)
        {
            if (z.Tree?.Right != null)
                return leftMostZip(Z(z.Tree.Right, P(S(z.Tree.Color, z.Tree.Value, Direction.ToRight, z.Tree.Left), z.Path)));

            var rp = rightMostZip(z);
            if (rp.Tree == null && rp.Path == null)
                return z.Tree == null ? z : Z(z.Tree.Right, P(S(z.Tree.Color, z.Tree.Value, Direction.ToRight, z.Tree.Left), z.Path));

            return rp;
        }

        public static a leftMostV<a>(a v, RBTree<a> t) => t == null ? v : leftMostV(t.Value, t.Left);

        public static RBTree<a> insert<a>(ROrder<a, Unit> ord, RBTree<a> t, a v) => setBlack(toTree(insertFixup(insertRedZip(ord, toZip(t), v))));

        public static RBZip<a> insertRedZip<a>(ROrder<a, Unit> ord, RBZip<a> z, a v) => z.Tree == null
            ? Z(N(Color.Red, v, null, null), z.Path)
            : (ord.Compare(v, z.Tree.Value).IsGt()
                ? insertRedZip(ord, Z(z.Tree.Right, P(S(z.Tree.Color, z.Tree.Value, Direction.ToRight, z.Tree.Left), z.Path)), v)
                : insertRedZip(ord, Z(z.Tree.Left, P(S(z.Tree.Color, z.Tree.Value, Direction.ToLeft, z.Tree.Right), z.Path)), v));


        public static RBZip<a> insertFixup<a>(RBZip<a> z) =>
            z.Tree.Color == Color.Red && z.Path?.Step.Color == Color.Red && z.Path?.Next?.Step.Color == Color.Black
                ? (z.Path.Next.Step.Tree.Color == Color.Red
                    ? insertFixupRBR(z.Tree, z.Path.Step.Value, z.Path.Step.Direction, z.Path.Step.Tree, z.Path.Next.Step.Value, z.Path.Next.Step.Direction, z.Path.Next.Step.Tree, z.Path.Next.Next)
                    : insertFixupRB(z.Tree, z.Tree.Value, z.Tree.Left, z.Tree.Right, z.Path.Step.Value, z.Path.Step.Direction, z.Path.Step.Tree, z.Path.Next.Step.Value, z.Path.Next.Step.Direction, z.Path.Next.Step.Tree, z.Path.Next.Next))
                : z;

        private static RBZip<a> insertFixupRBR<a>(RBTree<a> aa, a vb, Direction db, RBTree<a> sb, a vc, Direction dc, RBTree<a> d, Path<a> path)
        {
            var newD = setBlack(d);

            var newBL = db == Direction.ToLeft ? aa : sb;
            var newBR = db == Direction.ToRight ? sb : aa;

            var newB = N(Color.Black, vb, newBL, newBR);

            var newCL = dc == Direction.ToLeft ? newB : newD;
            var newCR = dc == Direction.ToLeft ? newD : newB;

            var newC = N(Color.Red, vc, newCL, newCR);

            return insertFixup(Z(newC, path));
        }

        private static RBZip<a> insertFixupRB<a>(RBTree<a> aa, a va, RBTree<a> sal, RBTree<a> sar, a vb, Direction db, RBTree<a> sb, a vc, Direction dc, RBTree<a> d, Path<a> path) =>
            dc == Direction.ToLeft
                ? (db == Direction.ToLeft
                    ? Z(aa, P(S(Color.Black, vb, dc, N(Color.Red, vc, sb, d)), path))
                    : Z(N(Color.Red, vb, sb, sal), P(S(Color.Black, va, dc, N(Color.Red, vc, sar, d)), path)))
                : (db == Direction.ToLeft
                    ? Z(N(Color.Red, vb, sar, sb), P(S(Color.Black, va, dc, N(Color.Red, vc, d, sal)), path))
                    : Z(aa, P(S(Color.Black, vb, dc, N(Color.Red, vc, d, sb)), path))
            );
    }
}
