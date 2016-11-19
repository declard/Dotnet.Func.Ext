namespace Dotnet.Func.Ext.Data
{
    using System;
    using static Orders;
    using static Units;
    using static Ctors;
    using static Exceptions;
    using static Core.Functions;
    using static Algebras.Structures;
    using Algebras;

    public static partial class Ctors
    {
        /// <summary>
        /// `Less than` ctor
        /// </summary>
        public static Ord Lt() => Ord.CreateLt();
        /// <summary>
        /// `Less than` injection
        /// </summary>
        public static Ord Lt(Unit u) => Ord.CreateLt();

        /// <summary>
        /// `Equals to` ctor
        /// </summary>
        public static Ord Eq() => Ord.CreateEq();
        /// <summary>
        /// `Equals to` injection
        /// </summary>
        public static Ord Eq(Unit u) => Ord.CreateEq();

        /// <summary>
        /// `Greater than` ctor
        /// </summary>
        public static Ord Gt() => Ord.CreateGt();
        /// <summary>
        /// `Greater than` injection
        /// </summary>
        public static Ord Gt(Unit u) => Ord.CreateGt();
    }

    public static partial class Dtors
    {
        /// <summary>
        /// `Less than` projection
        /// </summary>
        public static Unit Lt(this Ord that) =>
            that.Case(Id, Failing<Unit>.Tag(that, nameof(Lt)), Failing<Unit>.Tag(that, nameof(Lt)));
        /// <summary>
        /// `Equals to` projection
        /// </summary>
        public static Unit Eq(this Ord that) =>
            that.Case(Failing<Unit>.Tag(that, nameof(Lt)), Id, Failing<Unit>.Tag(that, nameof(Lt)));
        /// <summary>
        /// `Greater than` projection
        /// </summary>
        public static Unit Gt(this Ord that) =>
            that.Case(Failing<Unit>.Tag(that, nameof(Lt)), Failing<Unit>.Tag(that, nameof(Lt)), Id);

        /// <summary>
        /// `Less than` projection test
        /// </summary>
        public static bool IsLt(this Ord that) => that.Case(true, false, false);
        /// <summary>
        /// `Equals to` projection test
        /// </summary>
        public static bool IsEq(this Ord that) => that.Case(false, true, false);
        /// <summary>
        /// `Greater than` projection test
        /// </summary>
        public static bool IsGt(this Ord that) => that.Case(false, false, true);

    }

    /// <summary>
    /// Kinds of an order
    /// </summary>
    public static class Orders
    {
        /// <summary>
        /// Well-order: less than, equals to, greater than
        /// </summary>
        public struct Ord
        {
            private enum Ordˈ
            {
                Lt = -1,
                Eq = 0,
                Gt = 1,
            }

            private Ordˈ _ord;

            private Ord(Ordˈ ord) { _ord = ord; }
            
            public static Ord CreateLt() => new Ord(Ordˈ.Lt);
            public static Ord CreateEq() => new Ord(Ordˈ.Eq);
            public static Ord CreateGt() => new Ord(Ordˈ.Gt);

            /// <summary>
            /// Basic pattern matcher (closure-evading)
            /// </summary>
            public res Case<res, ltCtx, eqCtx, gtCtx>(ltCtx ltCtxˈ, Func<ltCtx, Unit, res> Lt, eqCtx eqCtxˈ, Func<eqCtx, Unit, res> Eq, gtCtx gtCtxˈ, Func<gtCtx, Unit, res> Gt)
            {
                switch (_ord)
                {
                    case Ordˈ.Lt: return Lt(ltCtxˈ, Unit());
                    case Ordˈ.Eq: return Eq(eqCtxˈ, Unit());
                    case Ordˈ.Gt: return Gt(gtCtxˈ, Unit());
                }

                throw EnumOutOfRangeException.Create(_ord);
            }

            public static Ord Compare(Ord left, Ord right) => left._ord.CompareTo(right._ord).ToOrd();

            public override string ToString() => this.Case("Lt()", "Eq()", "Gt()");
        }

        /// <summary>
        /// From neg/zero/pos integer to well-order
        /// </summary>
        public static Ord ToOrd(this int that) =>
           that > 0 ? Gt() :
           that < 0 ? Lt() :
           Eq();

        /// <summary>
        /// From well-order to neg/zero/pos integer
        /// </summary>
        public static int ToInt(this Ord that) => that.Case(-1, 0, 1);

        /// <summary>
        /// Not equal
        /// </summary>
        public static bool IsNeq(this Ord that) => that.IsEq().Not();

        /// <summary>
        /// Not less than
        /// </summary>
        public static bool IsNlt(this Ord that) => that.IsLt().Not();

        /// <summary>
        /// Not greater than
        /// </summary>
        public static bool IsNgt(this Ord that) => that.IsGt().Not();

        /// <summary>
        /// Basic pattern matcher
        /// </summary>
        public static res Case<res>(this Ord that, res Lt, res Eq, res Gt) =>
            that.Case(Lt, Fst, Eq, Fst, Gt, Fst);

        /// <summary>
        /// Basic pattern matcher (lazy)
        /// </summary>
        public static res Case<res>(this Ord that, Func<Unit, res> Lt, Func<Unit, res> Eq, Func<Unit, res> Gt) =>
            that.Case<Func<Unit, res>>(Lt, Eq, Gt).ToValue();

        /// <summary>
        /// Check whether value is on the range (including bounding points)
        /// </summary>
        public static bool IsOnBetween<v, a>(this v value, v lower, v upper, a alg) where a : ROrder<v, Unit> =>
            alg.Compare(value, lower).IsNlt() && alg.Compare(value, upper).IsNgt();

        /// <summary>
        /// Create well-order from inequalities
        /// </summary>
        public static Ord CreateOrd(bool isLess, bool isGreater) =>
             isLess && isGreater ? InvalidOperation<Ord>("`Less then` and `greater than` at the same time is not a well-order")
                 : ((isLess ? -1 : 0) + (isGreater ? 1 : 0)).ToOrd();
    }
}
