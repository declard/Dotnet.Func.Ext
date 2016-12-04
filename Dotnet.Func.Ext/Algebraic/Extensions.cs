
namespace Dotnet.Func.Ext.Algebraic
{
    using Data;
    using System;
    using static Data.Ctors;
    using static Data.Orders;
    using static Data.Units;
    using static Core.Functions;
    using static Signatures;

    public static class Extensions
    {
        public static Func<type, bool> ToPred<type>(this REquality<type, Unit> eq, type value) => e => eq.Equal(e, value);

        public static bool EqualByCompare<type>(Func<type, type, Ord> ord, type l, type r) => ord(l, r).IsEq();
        public static type InfByCompare<type>(Func<type, type, Ord> ord, type l, type r) => ord(l, r).Case(l, l, r);
        public static type SupByCompare<type>(Func<type, type, Ord> ord, type l, type r) => ord(l, r).Case(r, r, l);

        #region Human-readable Alg-based ops

        public static type Unit<type, mark>(this SNeutral<type, mark> neu) => neu.NullOp();
        public static type Inv<type, mark>(this SInv<type, mark> inv, type t) => inv.UnOp(t);
        public static type Op<type, mark>(this SSemigroup<type, mark> semi, type l, type r) => semi.BinOp(l, r);

        public static type Zero<type, mark>(this SNeutral<type, Additive<mark>> neu) => neu.Unit();
        public static type Neg<type, mark>(this SInv<type, Additive<mark>> inv, type t) => inv.Inv(t);
        public static type Add<type, mark>(this SSemigroup<type, Additive<mark>> semi, type l, type r) => semi.Op(l, r);
        public static type Sub<type, mark>(this SGroup<type, Additive<mark>> group, type l, type r) => group.Add(l, group.Neg(r));
        public static type Inc<type, mark>(this SRing<type, mark> ring, type t) => ring.Add(t, ring.One());
        public static type Dec<type, mark>(this SRing<type, mark> ring, type t) => ring.Sub(t, ring.One());

        public static type One<type, mark>(this SNeutral<type, Multiplicative<mark>> neu) => neu.Unit();
        public static type MinusOne<type, mark>(this SRing<type, mark> that) => that.Neg(that.One());
        public static type Recip<type, mark>(this SInv<type, Multiplicative<mark>> inv, type t) => inv.Inv(t);
        public static type Mul<type, mark>(this SSemigroup<type, Multiplicative<mark>> neu, type l, type r) => neu.Op(l, r);
        public static type Div<type, mark>(this SGroup<type, Multiplicative<mark>> group, type l, type r) => group.Mul(l, group.Recip(r));

        public static type Inf<type, mark>(this SSemigroup<type, Infimum<mark>> that, type l, type r) => that.Op(l, r);
        public static type Sup<type, mark>(this SSemigroup<type, Supremum<mark>> that, type l, type r) => that.Op(l, r);

        public static type Min<type, mark>(this SNeutral<type, Infimum<mark>> that) => that.Unit();
        public static type Max<type, mark>(this SNeutral<type, Supremum<mark>> that) => that.Unit();

        public static bool Equal<type, mark>(this REquality<type, mark> that, type l, type r) => that.BinOp(l, r);

        public static Ord Compare<type, mark>(this ROrder<type, mark> that, type l, type r)
        {
            SSemigroup<type, Infimum<mark>> semi = that;
            var inf = semi.Inf(l, r);
            return CreateOrd(that.Equal(r, inf).Not(), that.Equal(l, inf).Not());
        }


        // Because of multiple interface inheritance compiler can't decide what type arguments are
        // To be more specific, it can't deal with parameteric polymorphism of the second rank
        // so specialized extensions are provided for some operations
        public static type Zero<type>(this SNeutral<type, Additive<Unit>> neu) => neu.Unit();
        public static type One<type>(this SNeutral<type, Multiplicative<Unit>> neu) => neu.Unit();
        public static type Add<type>(this SSemigroup<type, Additive<Unit>> semi, type l, type r) => semi.Op(l, r);

        public static type Zero<type, mark>(this SRing<type, mark> ring) => ((SNeutral<type, Additive<mark>>)ring).Zero();
        public static type One<type, mark>(this SRing<type, mark> ring) => ((SNeutral<type, Multiplicative<mark>>)ring).One();
        public static type Add<type, mark>(this SRing<type, mark> ring, type l, type r) => ((SSemigroup<type, Additive<mark>>)ring).Add(l, r);

        #endregion

        #region *jections, homomorphisms and unnatural transformations

        public static type InjectLeft<type, left, right>(this SSumInj<type, left, right> CoAlg, left Left) =>
            CoAlg.Inject(Left, Either<Func<left, right>>.Left<Func<left, left>>(x => x));

        public static type InjectRight<type, left, right>(this SSumInj<type, left, right> CoAlg, right Right) =>
            CoAlg.Inject(Right, Either<Func<right, left>>.Right<Func<right, right>>(x => x));

        public static res Project<type, left, right, res>(this SSumProj<type, left, right> Alg, type Value, Func<left, res> Left, Func<right, res> Right) =>
            Alg.Project(Value, Pair(Left, Right));


        public static left ProjectLeft<type, left, right>(this SProdProj<type, left, right> Alg, type Value) =>
            Alg.Project(Value, Either<Func<right, left>>.Left<Func<left, left>>(x => x));

        public static right ProjectRight<type, left, right>(this SProdProj<type, left, right> Alg, type Value) =>
            Alg.Project(Value, Either<Func<left, right>>.Right<Func<right, right>>(x => x));

        public static res Inject<type, left, right, res>(this SProdInj<res, left, right> CoAlg, type Value, Func<type, left> Left, Func<type, right> Right) =>
            CoAlg.Inject(Value, Pair(Left, Right));


        public static typeTo Homo<typeFrom, typeTo, left, right>(this typeFrom From, SSumProj<typeFrom, left, right> Alg, SSumInj<typeTo, left, right> CoAlg) =>
            Alg.Project(From, CoAlg.InjectLeft, CoAlg.InjectRight);

        public static typeTo Homo<typeFrom, typeTo, left, right>(this typeFrom From, SProdProj<typeFrom, left, right> Alg, SProdInj<typeTo, left, right> CoAlg) =>
            CoAlg.Inject(From, Alg.ProjectLeft, Alg.ProjectRight);


        public static typeTo BiMap<typeFrom, typeTo, left, right, leftˈ, rightˈ>(this typeFrom From, SProdProj<typeFrom, left, right> Alg, Func<left, leftˈ> Left, Func<right, rightˈ> Right, SProdInj<typeTo, leftˈ, rightˈ> CoAlg) =>
            CoAlg.Inject(From, x => Left(Alg.ProjectLeft(x)), x => Right(Alg.ProjectRight(x)));

        public static typeTo BiMap<typeFrom, typeTo, left, right, leftˈ, rightˈ>(this typeFrom From, SSumProj<typeFrom, left, right> Alg, Func<left, leftˈ> Left, Func<right, rightˈ> Right, SSumInj<typeTo, leftˈ, rightˈ> CoAlg) =>
            Alg.Project(From, x => CoAlg.InjectLeft(Left(x)), x => CoAlg.InjectRight(Right(x)));

        public static typeTo FlipHomo<typeFrom, typeTo, left, right>(this typeFrom From, SSumProj<typeFrom, left, right> Alg, SSumInj<typeTo, right, left> CoAlg) =>
            Alg.Project(From, CoAlg.InjectRight, CoAlg.InjectLeft);

        public static typeTo FlipHomo<typeFrom, typeTo, left, right>(this typeFrom From, SProdProj<typeFrom, left, right> Alg, SProdInj<typeTo, right, left> CoAlg) =>
            CoAlg.Inject(From, Alg.ProjectRight, Alg.ProjectLeft);

        #endregion

        public static res Case<left, right, res>(this IEither<left, right> that, Func<left, res> Left, Func<right, res> Right) =>
            that.Case(Left, App, Right, App);
    }
}
