namespace Dotnet.Func.Ext.Algebraic
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
    using static Data.Functions;
    using static Extensions;
    using static Signatures;
    using static Core.Functions;

    [Resolvable]
    public class AInt32 : ROrder<int, Unit>, SRing<int, Unit>, SBounded<int, Unit>, SEnum<int>, SUnitInj<int, Unit>
    {
        public static readonly AInt32 Class = new AInt32();

        int SUnOp<int, int, Additive<Unit>>.UnOp(int t) => -t;

        int SNullOp<int, Multiplicative<Unit>>.NullOp() => 1;

        int SNullOp<int, Infimum<Unit>>.NullOp() => int.MinValue;
        int SNullOp<int, Supremum<Unit>>.NullOp() => int.MaxValue;

        int SBinOp<int, int, int, Multiplicative<Unit>>.BinOp(int l, int r) => l * r;

        int SNullOp<int, Additive<Unit>>.NullOp() => 0;

        int SBinOp<int, int, int, Additive<Unit>>.BinOp(int l, int r) => l + r;

        bool SBinOp<int, int, bool, Equative<Unit>>.BinOp(int l, int r) => EqualByCompare(Compare, l, r);
        int SBinOp<int, int, int, Infimum<Unit>>.BinOp(int l, int r) => InfByCompare(Compare, l, r);
        int SBinOp<int, int, int, Supremum<Unit>>.BinOp(int l, int r) => SupByCompare(Compare, l, r);

        Opt<int> SEnum<int>.Succ(int v) => (v != int.MaxValue).Then(v + 1);
        Opt<int> SEnum<int>.Pred(int v) => (v != int.MinValue).Then(v - 1);

        int SUnitInj<int, Unit>.Inj(Unit _) => 0;

        static Ord Compare(int l, int r) => l.CompareTo(r).ToOrd();
    }

    [Resolvable]
    public class AChar : ROrder<char, Unit>, SBounded<char, Unit>, SGroup<char, Additive<Unit>>, SNeutral<char, Multiplicative<Unit>>, SEnum<char>, SUnitInj<char, Unit>
    {
        public static readonly AChar Class = new AChar();

        char SNullOp<char, Infimum<Unit>>.NullOp() => char.MinValue;
        char SNullOp<char, Supremum<Unit>>.NullOp() => char.MaxValue;

        bool SBinOp<char, char, bool, Equative<Unit>>.BinOp(char l, char r) => EqualByCompare(Compare, l, r);
        char SBinOp<char, char, char, Infimum<Unit>>.BinOp(char l, char r) => InfByCompare(Compare, l, r);
        char SBinOp<char, char, char, Supremum<Unit>>.BinOp(char l, char r) => SupByCompare(Compare, l, r);

        char SNullOp<char, Additive<Unit>>.NullOp() => (char)0;
        char SUnOp<char, char, Additive<Unit>>.UnOp(char t) => (char)(-t);
        char SBinOp<char, char, char, Additive<Unit>>.BinOp(char l, char r) => (char)(l + r);

        char SNullOp<char, Multiplicative<Unit>>.NullOp() => (char)1;

        Opt<char> SEnum<char>.Succ(char v) => (v != char.MaxValue).Then((char)(v + 1));
        Opt<char> SEnum<char>.Pred(char v) => (v != char.MinValue).Then((char)(v - 1));

        char SUnitInj<char, Unit>.Inj(Unit _) => (char)0;

        static Ord Compare(char l, char r) => l.CompareTo(r).ToOrd();
    }

    [Resolvable]
    public class ARatio : SField<Ratio, Unit>, ROrder<Ratio, Unit>, SUnitInj<Ratio, Unit>
    {
        public static readonly ARatio Class = new ARatio();

        Ratio SNullOp<Ratio, Additive<Unit>>.NullOp() => Ratio.Zero;
        Ratio SBinOp<Ratio, Ratio, Ratio, Additive<Unit>>.BinOp(Ratio l, Ratio r) => Ratio.Add(l, r);
        Ratio SUnOp<Ratio, Ratio, Additive<Unit>>.UnOp(Ratio t) => Ratio.Neg(t);

        Ratio SNullOp<Ratio, Multiplicative<Unit>>.NullOp() => Ratio.One;
        Ratio SBinOp<Ratio, Ratio, Ratio, Multiplicative<Unit>>.BinOp(Ratio l, Ratio r) => Ratio.Mul(l, r);
        Ratio SUnOp<Ratio, Ratio, Multiplicative<Unit>>.UnOp(Ratio t) => Ratio.Inv(t);

        bool SBinOp<Ratio, Ratio, bool, Equative<Unit>>.BinOp(Ratio l, Ratio r) => EqualByCompare(Compare, l, r);
        Ratio SBinOp<Ratio, Ratio, Ratio, Infimum<Unit>>.BinOp(Ratio l, Ratio r) => SupByCompare(Compare, l, r);
        Ratio SBinOp<Ratio, Ratio, Ratio, Supremum<Unit>>.BinOp(Ratio l, Ratio r) => InfByCompare(Compare, l, r);

        Ratio SUnitInj<Ratio, Unit>.Inj(Unit Value) => Ratio.Zero;

        static Ord Compare(Ratio l, Ratio r) => AInt32.Class.Compare(l.Numerator * r.Denominator, r.Numerator * l.Denominator);
    }

    [Resolvable]
    public class ADouble: SField<double, Unit>, ROrder<double, Unit>, SUnitInj<double, Unit>
    {
        public static readonly ADouble Class = new ADouble();

        double SNullOp<double, Additive<Unit>>.NullOp() => 0;
        double SBinOp<double, double, double, Additive<Unit>>.BinOp(double l, double r) => l + r;
        double SUnOp<double, double, Additive<Unit>>.UnOp(double t) => -t;

        double SNullOp<double, Multiplicative<Unit>>.NullOp() => 1;
        double SBinOp<double, double, double, Multiplicative<Unit>>.BinOp(double l, double r) => l * r;
        double SUnOp<double, double, Multiplicative<Unit>>.UnOp(double t) => 1 / t;

        bool SBinOp<double, double, bool, Equative<Unit>>.BinOp(double l, double r) => EqualByCompare(Compare, l, r);
        double SBinOp<double, double, double, Infimum<Unit>>.BinOp(double l, double r) => InfByCompare(Compare, l, r);
        double SBinOp<double, double, double, Supremum<Unit>>.BinOp(double l, double r) => SupByCompare(Compare, l, r);

        double SUnitInj<double, Unit>.Inj(Unit _) => 0;

        static Ord Compare(double l, double r) => l.CompareTo(r).ToOrd();
    }

    [Resolvable]
    public class ABool : ROrder<bool, Unit>, SBounded<bool, Unit>, SSumProj<bool, Unit, Unit>, SSumInj<bool, Unit, Unit>, SEnum<bool>
    {
        public static readonly ABool Class = new ABool();

        bool SNullOp<bool, Infimum<Unit>>.NullOp() => false;
        bool SNullOp<bool, Supremum<Unit>>.NullOp() => true;

        bool SBinOp<bool, bool, bool, Equative<Unit>>.BinOp(bool l, bool r) => EqualByCompare(Compare, l, r);
        bool SBinOp<bool, bool, bool, Infimum<Unit>>.BinOp(bool l, bool r) => SupByCompare(Compare, l, r);
        bool SBinOp<bool, bool, bool, Supremum<Unit>>.BinOp(bool l, bool r) => InfByCompare(Compare, l, r);
        
        public res Project<res>(bool Value, Pair<Func<Unit, res>, Func<Unit, res>> Projector) =>
            Value ? Projector.Right()(Unit()) : Projector.Left()(Unit());

        public bool Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, Unit>> Injector) =>
            Injector.Case(Arg, (a, f) => Seq(f(a), false), Arg, (a, f) => Seq(f(a), true));

        public Opt<bool> Succ(bool v) => v ? None<bool>() : Some(true);

        public Opt<bool> Pred(bool v) => v ? Some(false) : None<bool>();

        static Ord Compare(bool l, bool r) => l.CompareTo(r).ToOrd();
    }

    [Resolvable]
    public class AString : REquality<string, Unit>, SMonoid<string, Additive<Unit>>, SList<string, char>, SUnitInj<string, char>
    {
        public static readonly AString Class = new AString();

        string SNullOp<string, Additive<Unit>>.NullOp() => "";
        string SBinOp<string, string, string, Additive<Unit>>.BinOp(string l, string r) => Default(l) + Default(r);

        bool SBinOp<string, string, bool, Equative<Unit>>.BinOp(string l, string r) => ReferenceEquals(l, r) || string.Equals(l, r);

        string SSumInj<string, Unit, Pair<char, string>>.Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, Pair<char, string>>> Injector) =>
            Injector.Case(Arg, (a, f) => Seq(f(a), ""), Arg, (a, f) => InjectRight(f(a)));

        private static string InjectRight(Pair<char, string> Right) => Right.Left() + Default(Right.Right());

        private static string Default(string v) => v ?? "";

        res SSumProj<string, Unit, Pair<char, string>>.Project<res>(string Value, Pair<Func<Unit, res>, Func<Pair<char, string>, res>> Projector) =>
            string.IsNullOrEmpty(Value) ? Projector.Left()(Unit()) : Projector.Right()(Pair(Value[0], Value.Substring(1)));

        string SUnitInj<string, char>.Inj(char Value) => Value.ToString();
    }

    [Resolvable]
    public class AOrd : ROrder<Ord, Unit>, SBounded<Ord, Unit>, SEnum<Ord>
    {
        public static readonly AOrd Class = new AOrd();

        Ord SNullOp<Ord, Infimum<Unit>>.NullOp() => Ord.CreateLt();
        Ord SNullOp<Ord, Supremum<Unit>>.NullOp() => Ord.CreateGt();

        bool SBinOp<Ord, Ord, bool, Equative<Unit>>.BinOp(Ord l, Ord r) => EqualByCompare(Ord.Compare, l, r);
        Ord SBinOp<Ord, Ord, Ord, Infimum<Unit>>.BinOp(Ord l, Ord r) => SupByCompare(Ord.Compare, l, r);
        Ord SBinOp<Ord, Ord, Ord, Supremum<Unit>>.BinOp(Ord l, Ord r) => InfByCompare(Ord.Compare, l, r);

        public Opt<Ord> Succ(Ord v) => v.Case(Some(Eq()), Some(Gt()), None<Ord>());
        public Opt<Ord> Pred(Ord v) => v.Case(None<Ord>(), Some(Lt()), Some(Eq()));
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
    public class AEither<left, right> : SSumProj<Either<left, right>, left, right>, SSumInj<Either<left, right>, left, right>, SUnitInj<Either<left, right>, right>
    {
        public static readonly AEither<left, right> Class = new AEither<left, right>();

        public Either<left, right> Inject<arg>(arg Arg, Either<Func<arg, left>, Func<arg, right>> Injector) =>
            Injector.Case(Arg, (a, f) => Either<right>.Left(f(a)), Arg, (a, f) => Either<left>.Right(f(a)));

        public res Project<res>(Either<left, right> Value, Pair<Func<left, res>, Func<right, res>> Projector) =>
            Value.Case(Projector.Left(), Projector.Right());

        public AEither<right, left> Flip() => AEither<right, left>.Class;

        Either<left, right> SUnitInj<Either<left, right>, right>.Inj(right Value) => Either<left>.Right(Value);
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
    public class AOpt<val> : SSumProj<Opt<val>, Unit, val>, SSumInj<Opt<val>, Unit, val>, SUnitInj<Opt<val>, val>
    {
        public static readonly AOpt<val> Class = new AOpt<val>();

        public Opt<val> Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, val>> Injector) =>
            Injector.Case(Arg, (a, f) => None<val>(f(a)), Arg, (a, f) => Some(f(a)));

        public res Project<res>(Opt<val> Value, Pair<Func<Unit, res>, Func<val, res>> Projector) =>
            Value.Case(Projector.Left(), Projector.Right());

        Opt<val> SUnitInj<Opt<val>, val>.Inj(val Value) => Some(Value);
    }
    
    public class AOptMonoidLeft<val, mark> : AOpt<val>, SMonoid<Opt<val>, mark>
    {
        Opt<val> SBinOp<Opt<val>, Opt<val>, Opt<val>, mark>.BinOp(Opt<val> l, Opt<val> r) =>
            l.Map(Some).GetValueOr(r);

        Opt<val> SNullOp<Opt<val>, mark>.NullOp() => None<val>();
    }
    
    public class AOptMonoidRight<val, mark> : AOpt<val>, SMonoid<Opt<val>, mark>
    {
        Opt<val> SBinOp<Opt<val>, Opt<val>, Opt<val>, mark>.BinOp(Opt<val> l, Opt<val> r) =>
            r.Map(Some).GetValueOr(l);

        Opt<val> SNullOp<Opt<val>, mark>.NullOp() => None<val>();
    }

    public class AOptMonoid<val, mark> : AOpt<val>, SMonoid<Opt<val>, mark>
    {
        SSemigroup<val, mark> _semi;

        public AOptMonoid(SSemigroup<val, mark> semi) { _semi = semi; }

        Opt<val> SBinOp<Opt<val>, Opt<val>, Opt<val>, mark>.BinOp(Opt<val> l, Opt<val> r) =>
            l.Case(r, Fst, r, (rˈ, lv) => Some(rˈ.Case(lv, Fst, lv, (lvˈ, rv) => _semi.BinOp(lvˈ, rv))));

        Opt<val> SNullOp<Opt<val>, mark>.NullOp() => None<val>();
    }

    public class AOptEq<val, mark> : REquality<Opt<val>, mark>
    {
        REquality<val, mark> _eq;

        public AOptEq(REquality<val, mark> eq)
        {
            _eq = eq;
        }

        bool SBinOp<Opt<val>, Opt<val>, bool, Equative<mark>>.BinOp(Opt<val> l, Opt<val> r) =>
            l.IsNone() == r.IsNone() && Optionals.Lift(_eq.Equal, l, r).GetValueOr(true);
    }

    [Resolvable]
    public class AList<val> :
        SMonoid<Lists.List<val>, Additive<Unit>>,
        SUnitInj<Lists.List<val>, val>,
        SList<Lists.List<val>, val>
    {
        public static readonly AList<val> Class = new AList<val>();

        Lists.List<val> SNullOp<Lists.List<val>, Additive<Unit>>.NullOp() => Nil<val>();

        public Lists.List<val> Inj(val from) => Lists.List.Pure(from);

        public Lists.List<val> Inject<arg>(arg Arg, Either<Func<arg, Unit>, Func<arg, Pair<val, Lists.List<val>>>> Injector) =>
            Injector.Case(Arg, (a, f) => Nil<val>(f(a)), Arg, (a, f) => InjectRight(f(a)));
        
        private static Lists.List<val> InjectRight(Pair<val, Lists.List<val>> Right) => Cons(Right.Left(), Right.Right());

        public res Project<res>(Lists.List<val> Value, Pair<Func<Unit, res>, Func<Pair<val, Lists.List<val>>, res>> Projector) =>
            Value.Case(Projector.Left(), Projector.Right());

        Lists.List<val> SBinOp<Lists.List<val>, Lists.List<val>, Lists.List<val>, Additive<Unit>>.BinOp(Lists.List<val> l, Lists.List<val> r) =>
            l.Append(r);
    }

    public class AEqComparer<val> : IEqualityComparer<val>, REquality<val, Unit>
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

        bool SBinOp<val, val, bool, Equative<Unit>>.BinOp(val l, val r) => Equals(l, r);
    }

    public static class AEqComparer
    {
        public static AEqComparer<val> Create<val>(Func<val, val, bool> eq, Func<val, int> hash) =>
            new AEqComparer<val>(eq, hash);

        public static AEqComparer<val> Create<val>(Func<val, val, bool> eq) =>
            new AEqComparer<val>(eq, v => v.GetHashCode());
    }

    public class AOrdComparer<val> : IComparer<val>, ROrder<val, Unit>
    {
        Func<val, val, Ord> _ord;

        public AOrdComparer(Func<val, val, Ord> ord)
        {
            _ord = ord;
        }

        public int Compare(val x, val y) => _ord(x, y).ToInt();

        val SBinOp<val, val, val, Supremum<Unit>>.BinOp(val l, val r) => SupByCompare(_ord, l, r);
        val SBinOp<val, val, val, Infimum<Unit>>.BinOp(val l, val r) => InfByCompare(_ord, l, r);
        bool SBinOp<val, val, bool, Equative<Unit>>.BinOp(val l, val r) => EqualByCompare(_ord, l, r);
    }

    public static class AOrdComparer
    {
        public static AOrdComparer<val> Create<val>(Func<val, val, Ord> ord) =>
            new AOrdComparer<val>(ord);
    }

    public class AOrderable<val> : ROrder<val, Unit>
    {
        private Func<val, val, Ord> _c;

        public static ROrder<val, Unit> Create(Func<val, val, Ord> c) =>
            new AOrderable<val> { _c = c };

        val SBinOp<val, val, val, Supremum<Unit>>.BinOp(val l, val r) => SupByCompare(_c, l, r);
        val SBinOp<val, val, val, Infimum<Unit>>.BinOp(val l, val r) => InfByCompare(_c, l, r);
        bool SBinOp<val, val, bool, Equative<Unit>>.BinOp(val l, val r) => EqualByCompare(_c, l, r);
    }

    public static class DefaultAlgebras
    {
        public static AEither<left, right> Alg<left, right>(this Either<left, right> _) => AEither<left, right>.Class;
        public static APair<left, right> Alg<left, right>(this Pair<left, right> _) => APair<left, right>.Class;
        public static AOpt<val> Alg<val>(this Opt<val> _) => AOpt<val>.Class;
        public static ABool Alg(this bool _) => ABool.Class;
        public static AOrd Alg(this Ord _) => AOrd.Class;
        public static AList<val> Alg<val>(this Lists.List<val> _) => AList<val>.Class;
        public static AString Alg(this string _) => AString.Class;
        public static AChar Alg(this char _) => AChar.Class;
    }
}
