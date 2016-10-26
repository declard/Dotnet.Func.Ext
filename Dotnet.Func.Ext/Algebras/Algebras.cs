namespace Dotnet.Func.Ext.Algebras
{
    using Data;
    using System;
    using System.Collections.Generic;
    using static Data.Ctors;
    using static Data.Eithers;
    using static Data.Optionals;
    using static Data.Orders;
    using static Data.Tuples;
    using static Data.Units;
    using static Extensions;
    using static Functions;
    using static Structures;

    [Resolvable]
    public class AInt32 : SNat<int>, ROrderByCompare<int, Unit>, SRing<int, Unit>, SBounded<int, Unit>
    {
        public static readonly AInt32 Class = new AInt32();

        int SUnOp<int, int, Additive<Unit>>.UnOp(int t) => -t;

        int SNullOp<int, Multiplicative<Unit>>.NullOp() => 1;

        int SNullOp<int, Infimum<Unit>>.NullOp() => int.MinValue;
        int SNullOp<int, Supremum<Unit>>.NullOp() => int.MaxValue;

        int SBinOp<int, int, int, Multiplicative<Unit>>.BinOp(int l, int r) => l * r;

        int SNullOp<int, Additive<Unit>>.NullOp() => 0;

        int SBinOp<int, int, int, Additive<Unit>>.BinOp(int l, int r) => l + r;

        Ord ROrderByCompare<int, Unit>.Compare(int l, int r) => l.CompareTo(r).ToOrd();
        bool SBinOp<int, int, bool, Equative<Unit>>.BinOp(int l, int r) => EqualByCompare(this, l, r);
        int SBinOp<int, int, int, Infimum<Unit>>.BinOp(int l, int r) => InfByCompare(this, l, r);
        int SBinOp<int, int, int, Supremum<Unit>>.BinOp(int l, int r) => SupByCompare(this, l, r);
        
        public res Project<res>(int Value, Pair<Func<Unit, res>, Func<int, res>> Projector) =>
            Value == 0 ? Projector.Left()(Unit()) : Projector.Right()(Value - 1);
        public int Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, int>> Injector) =>
            Injector.Case(Arg, (a, f) => Seq(f(a), 0), Arg, (a, f) => f(a) + 1);
    }

    [Resolvable]
    public class ARatio : SField<Ratio, Unit>, ROrderByCompare<Ratio, Unit>, SNat<Ratio>
    {
        public static readonly ARatio Class = new ARatio();

        Ratio SNullOp<Ratio, Additive<Unit>>.NullOp() => Ratio.Zero;
        Ratio SBinOp<Ratio, Ratio, Ratio, Additive<Unit>>.BinOp(Ratio l, Ratio r) => Ratio.Add(l, r);
        Ratio SUnOp<Ratio, Ratio, Additive<Unit>>.UnOp(Ratio t) => Ratio.Neg(t);

        Ratio SNullOp<Ratio, Multiplicative<Unit>>.NullOp() => Ratio.One;
        Ratio SBinOp<Ratio, Ratio, Ratio, Multiplicative<Unit>>.BinOp(Ratio l, Ratio r) => Ratio.Mul(l, r);
        Ratio SUnOp<Ratio, Ratio, Multiplicative<Unit>>.UnOp(Ratio t) => Ratio.Inv(t);

        Ord ROrderByCompare<Ratio, Unit>.Compare(Ratio l, Ratio r) => AInt32.Class.Compare(l.Numerator * r.Denominator, r.Numerator * l.Denominator);
        bool SBinOp<Ratio, Ratio, bool, Equative<Unit>>.BinOp(Ratio l, Ratio r) => EqualByCompare(this, l, r);
        Ratio SBinOp<Ratio, Ratio, Ratio, Infimum<Unit>>.BinOp(Ratio l, Ratio r) => SupByCompare(this, l, r);
        Ratio SBinOp<Ratio, Ratio, Ratio, Supremum<Unit>>.BinOp(Ratio l, Ratio r) => InfByCompare(this, l, r);

        public res Project<res>(Ratio Value, Pair<Func<Unit, res>, Func<Ratio, res>> Projector) =>
            this.Equal(Value.Normalize(), Ratio.Zero) ? Projector.Left()(Unit()) : Projector.Right()(Ratio.Add(Value, Ratio.Neg(Ratio.One)));

        public Ratio Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, Ratio>> Injector) =>
            Injector.Case(Arg, (a, f) => Seq(f(a), Ratio.Zero), Arg, (a, f) => Ratio.Add(f(a), Ratio.One));
    }

    [Resolvable]
    public class ADouble: SField<double, Unit>, ROrderByCompare<double, Unit>, SNat<double>
    {
        public static readonly ADouble Class = new ADouble();

        double SNullOp<double, Additive<Unit>>.NullOp() => 0;
        double SBinOp<double, double, double, Additive<Unit>>.BinOp(double l, double r) => l + r;
        double SUnOp<double, double, Additive<Unit>>.UnOp(double t) => -t;

        double SNullOp<double, Multiplicative<Unit>>.NullOp() => 1;
        double SBinOp<double, double, double, Multiplicative<Unit>>.BinOp(double l, double r) => l * r;
        double SUnOp<double, double, Multiplicative<Unit>>.UnOp(double t) => 1 / t;

        Ord ROrderByCompare<double, Unit>.Compare(double l, double r) => l.CompareTo(r).ToOrd();
        bool SBinOp<double, double, bool, Equative<Unit>>.BinOp(double l, double r) => EqualByCompare(this, l, r);
        double SBinOp<double, double, double, Infimum<Unit>>.BinOp(double l, double r) => InfByCompare(this, l, r);
        double SBinOp<double, double, double, Supremum<Unit>>.BinOp(double l, double r) => SupByCompare(this, l, r);
        
        public res Project<res>(double Value, Pair<Func<Unit, res>, Func<double, res>> Projector) =>
            Value == 0 ? Projector.Left()(Unit()) : Projector.Right()(Value - 1);

        public double Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, double>> Injector) =>
            Injector.Case(Arg, (a, f) => Seq(f(a), 0), Arg, (a, f) => f(a) + 1);
    }

    [Resolvable]
    public class ABool : ROrderByCompare<bool, Unit>, SBounded<bool, Unit>, SSumProj<bool, Unit, Unit>, SSumInj<bool, Unit, Unit>
    {
        public static readonly ABool Class = new ABool();

        bool SNullOp<bool, Infimum<Unit>>.NullOp() => false;
        bool SNullOp<bool, Supremum<Unit>>.NullOp() => true;

        Ord ROrderByCompare<bool, Unit>.Compare(bool l, bool r) => l.CompareTo(r).ToOrd();
        bool SBinOp<bool, bool, bool, Equative<Unit>>.BinOp(bool l, bool r) => EqualByCompare(this, l, r);
        bool SBinOp<bool, bool, bool, Infimum<Unit>>.BinOp(bool l, bool r) => SupByCompare(this, l, r);
        bool SBinOp<bool, bool, bool, Supremum<Unit>>.BinOp(bool l, bool r) => InfByCompare(this, l, r);
        
        public res Project<res>(bool Value, Pair<Func<Unit, res>, Func<Unit, res>> Projector) =>
            Value ? Projector.Right()(Unit()) : Projector.Left()(Unit());

        public bool Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, Unit>> Injector) =>
            Injector.Case(Arg, (a, f) => Seq(f(a), false), Arg, (a, f) => Seq(f(a), true));
    }

    [Resolvable]
    public class AOrd : ROrderByCompare<Ord, Unit>, SBounded<Ord, Unit>
    {
        public static readonly AOrd Class = new AOrd();

        Ord SNullOp<Ord, Infimum<Unit>>.NullOp() => Ord.CreateLt();
        Ord SNullOp<Ord, Supremum<Unit>>.NullOp() => Ord.CreateGt();

        Ord ROrderByCompare<Ord, Unit>.Compare(Ord l, Ord r) => Ord.Compare(l, r);
        bool SBinOp<Ord, Ord, bool, Equative<Unit>>.BinOp(Ord l, Ord r) => EqualByCompare(this, l, r);
        Ord SBinOp<Ord, Ord, Ord, Infimum<Unit>>.BinOp(Ord l, Ord r) => SupByCompare(this, l, r);
        Ord SBinOp<Ord, Ord, Ord, Supremum<Unit>>.BinOp(Ord l, Ord r) => InfByCompare(this, l, r);
    }

    [Resolvable]
    public class AIEither<either, left, right> : SSumProj<either, left, right>
        where either : IEither<left, right>
    {
        public static readonly AIEither<either, left, right> Class = new AIEither<either, left, right>();
        
        public res Project<res>(either Value, Pair<Func<left, res>, Func<right, res>> Projector) =>
            Value.Case(Projector.Left(), Projector.Right());
    }

    [Resolvable]
    public class AEither<left, right> : SSumProj<Either<left, right>, left, right>, SSumInj<Either<left, right>, left, right>
    {
        public static readonly AEither<left, right> Class = new AEither<left, right>();

        public Either<left, right> Inject<arg>(arg Arg, Either<Func<arg, left>, Func<arg, right>> Injector) =>
            Injector.Case(Arg, (a, f) => Either<right>.Left(f(a)), Arg, (a, f) => Either<left>.Right(f(a)));

        public res Project<res>(Either<left, right> Value, Pair<Func<left, res>, Func<right, res>> Projector) =>
            Value.Case(Projector.Left(), Projector.Right());

        public AEither<right, left> Flip() => AEither<right, left>.Class;
    }
    
    [Resolvable]
    public class APair<left, right> : SProdInj<Pair<left, right>, left, right>, SProdProj<Pair<left, right>, left, right>
    {
        public static readonly APair<left, right> Class = new APair<left, right>();

        public Pair<left, right> Inject<arg>(arg Arg, Pair<Func<arg, left>, Func<arg, right>> Injector) =>
            Pair(Injector.Left()(Arg), Injector.Right()(Arg));

        public res Project<res>(Pair<left, right> Value, Either<Func<left, res>, Func<right, res>> Projector) =>
            Projector.Case(Value, (a, f) => f(a.Left()), Value, (a, f) => f(a.Right()));

        public APair<right, left> Flip() => APair<right, left>.Class;
    }

    [Resolvable]
    public class AOpt<val> : SSumProj<Opt<val>, Unit, val>, SSumInj<Opt<val>, Unit, val>
    {
        public static readonly AOpt<val> Class = new AOpt<val>();

        public Opt<val> Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, val>> Injector) =>
            Injector.Case(Arg, (a, f) => None<val>(f(a)), Arg, (a, f) => Some(f(a)));

        public res Project<res>(Opt<val> Value, Pair<Func<Unit, res>, Func<val, res>> Projector) =>
            Value.Case(Projector.Left(), Projector.Right());
    }

    [Resolvable]
    public class AList<val> :
        SMonoid<Lists.List<val>, Additive<Unit>>,
        SUnitInj<Lists.List<val>, val>,
        SSumInj<Lists.List<val>, Unit, Pair<val, Lists.List<val>>>,
        SSumProj<Lists.List<val>, Unit, Pair<val, Lists.List<val>>>
    {
        public static readonly AList<val> Class = new AList<val>();

        Lists.List<val> SNullOp<Lists.List<val>, Additive<Unit>>.NullOp() => Nil<val>();

        public Lists.List<val> Inj(val from) => Lists.List.Pure(from);

        public Lists.List<val> Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, Pair<val, Lists.List<val>>>> Injector) =>
            Injector.Case(Arg, (a, f) => InjectLeft(f(a)), Arg, (a, f) => InjectRight(f(a)));

        public Lists.List<val> InjectLeft(Unit Left) => Nil<val>();

        public Lists.List<val> InjectRight(Pair<val, Lists.List<val>> Right) => Cons(Right.Left(), Right.Right());

        public res Project<res>(Lists.List<val> Value, Pair<Func<Unit, res>, Func<Pair<val, Lists.List<val>>, res>> Projector) =>
            Value.Case(Projector.Left(), Projector.Right());

        Lists.List<val> SBinOp<Lists.List<val>, Lists.List<val>, Lists.List<val>, Additive<Unit>>.BinOp(Lists.List<val> l, Lists.List<val> r) =>
            l.Append(r);
    }

    public class AEqComparer<val> : IEqualityComparer<val>
    {
        Func<val, val, bool> _eq;
        Func<val, int> _hash;

        public AEqComparer(Func<val, val, bool> eq, Func<val, int> hash)
        {
            _eq = eq;
            _hash = hash;
        }

        public bool Equals(val x, val y) => _eq(x, y);
        public int GetHashCode(val obj) => _hash(obj);
    }

    public static class AEqComparer
    {
        public static AEqComparer<val> Create<val>(Func<val, val, bool> eq, Func<val, int> hash) =>
            new AEqComparer<val>(eq, hash);

        public static AEqComparer<val> Create<val>(Func<val, val, bool> eq) =>
            new AEqComparer<val>(eq, v => v.GetHashCode());
    }

    public class AOrdComparer<val> : IComparer<val>
    {
        Func<val, val, int> _ord;

        public AOrdComparer(Func<val, val, int> ord)
        {
            _ord = ord;
        }

        public int Compare(val x, val y) => _ord(x, y);
    }

    public static class AOrdComparer
    {
        public static AOrdComparer<val> Create<val>(Func<val, val, int> ord) =>
            new AOrdComparer<val>(ord);
    }

    public static class DefaultAlgebras
    {
        public static AEither<left, right> Alg<left, right>(this Either<left, right> _) => AEither<left, right>.Class;
        public static APair<left, right> Alg<left, right>(this Pair<left, right> _) => APair<left, right>.Class;
        public static AOpt<val> Alg<val>(this Opt<val> _) => AOpt<val>.Class;
        public static ABool Alg(this bool _) => ABool.Class;
        public static AOrd Alg(this Ord _) => AOrd.Class;
        public static AList<val> Alg<val>(this Lists.List<val> _) => AList<val>.Class;
    }
}
