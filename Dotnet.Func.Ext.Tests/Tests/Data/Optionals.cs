using Dotnet.Func.Ext.Data;
using NUnit.Framework;
using System;
using static Dotnet.Func.Ext.Core.Functions;
using static Dotnet.Func.Ext.Data.Ctors;
using static Dotnet.Func.Ext.Data.Optionals;

namespace Dotnet.Func.Ext.Tests.Tests.Data
{
    [TestFixture]
    public class TOptionals
    {
        private void AssertEq<val>(Opt<val> l, Opt<val> r) => Assert.AreEqual(l, r);

        [Test]
        public void Default()
        {
            AssertEq(None<int>(), default(Opt<int>));
        }

        [Test]
        public void Pure()
        {
            var v = 1;
            AssertEq(Some(v), v.PureOpt());
        }

        #region Laws

        // None |> map id = None
        [Test]
        public void FunctorIdNone()
        {
            var a = None<int>();
            AssertEq(a.Map(Id), a);
        }

        // Some(a) |> map id = Some(a)
        [Test]
        public void FunctorIdSome()
        {
            var a = Some(1);
            AssertEq(a.Map(Id), a);
        }

        // map (g.h) None = None |> map h |> map g
        [Test]
        public void FunctorCompNone()
        {
            var a = None<int>();
            var h = Func<int, int>(x => x + 3);
            var g = Func<int, int>(x => x * 3);
            AssertEq(a.Map(g.o(h)), a.Map(h).Map(g));
        }

        // map (g.h) Some(a) = Some(a) |> map h |> map g
        [Test]
        public void FunctorCompSome()
        {
            var a = Some(1);
            var h = Func<int, int>(x => x + 3);
            var g = Func<int, int>(x => x * 3);
            AssertEq(a.Map(g.o(h)), a.Map(h).Map(g));
        }

        // pure v >>= f = f v | f = λ_ -> None
        [Test]
        public void MonadOutterIdNone()
        {
            var v = 1;
            var f = Func<int, Opt<int>>(x => None<int>());
            AssertEq(v.PureOpt().FlatMap(f), f(v));
        }

        // pure v >>= f = f v | f = λa -> Some(a)
        [Test]
        public void MonadOutterIdSome()
        {
            var v = 1;
            var f = Func<int, int>(x => x + 3).Map(Opt.Pure);
            AssertEq(v.PureOpt().FlatMap(f), f(v));
        }

        // None >>= pure = None
        [Test]
        public void MonadInnerIdNone()
        {
            var a = None<int>();
            AssertEq(a.FlatMap(Opt.Pure), a);
        }

        // Some(a) >>= id = Some(a)
        [Test]
        public void MonadInnerIdSome()
        {
            var a = Some(1);
            AssertEq(a.FlatMap(Opt.Pure), a);
        }

        // None >>= h >>= g = None >>= \x -> h x >>= g
        [Test]
        public void MonadAssocNone()
        {
            var a = None<int>();
            var g = Func<int, int>(x => x + 3).Map(Opt.Pure);
            var h = Func<int, int>(x => x * 3).Map(Opt.Pure);
            AssertEq(a.FlatMap(h).FlatMap(g), a.FlatMap(x => h(x).FlatMap(g)));
        }

        // Some(a) >>= h >>= g = Some(a) >>= \x -> h x >>= g
        [Test]
        public void MonadAssocSome()
        {
            var a = Some(1);
            var g = Func<int, int>(x => x + 3).Map(Opt.Pure);
            var h = Func<int, int>(x => x * 3).Map(Opt.Pure);
            AssertEq(a.FlatMap(h).FlatMap(g), a.FlatMap(x => h(x).FlatMap(g)));
        }

        // pure id <*> None = None
        [Test]
        public void ApplicativeIdNone()
        {
            var v = None<int>();
            AssertEq(Opt.Pure<Func<int, int>>(Id).Ap(v), v);
        }

        // pure id <*> Some(a) = Some(a)
        [Test]
        public void ApplicativeIdSome()
        {
            var v = Some(1);
            AssertEq(Opt.Pure<Func<int, int>>(Id).Ap(v), v);
        }

        // pure (.) <*> u <*> v <*> w = u <*> (v <*> w)
        [Test, Combinatorial]
        public void ApplicativeComp(
            [Values(false, true)] bool hasU,
            [Values(false, true)] bool hasV,
            [Values(false, true)] bool hasW)
        {
            var u = Func<int, int>(x => x + 3).PureOpt().Filter(_ => hasU);
            var v = Func<int, int>(x => x * 3).PureOpt().Filter(_ => hasV);
            var w = Some(1).Filter(_ => hasW);
            AssertEq(Curry<Func<int, int>, Func<int, int>, Func<int, int>>(Compose).PureOpt().Ap(u).Ap(v).Ap(w), u.Ap(v.Ap(w)));
        }

        // pure f <*> pure v = pure (f v)
        [Test]
        public void ApplicativeHomo()
        {
            var f = Func<int, int>(x => x + 3);
            var v = 2;
            AssertEq(f.PureOpt().Ap(v.PureOpt()), f(v).PureOpt());
        }

        // None <*> pure v = pure ($ v) <*> None
        [Test]
        public void ApplicativeCommutNone()
        {
            var u = None<Func<int, int>>();
            var v = 2;
            AssertEq(u.Ap(v.PureOpt()), Func<Func<int, int>, int>(x => x(v)).PureOpt().Ap(u));
        }

        // Some(f) <*> pure v = pure ($ v) <*> Some(f)
        [Test]
        public void ApplicativeCommut()
        {
            var u = Func<int, int>(x => x + 3).PureOpt();
            var v = 2;
            AssertEq(u.Ap(v.PureOpt()), Func<Func<int, int>, int>(x => x(v)).PureOpt().Ap(u));
        }

        #endregion

        [Test]
        public void FilterNone()
        {
            AssertEq(None<bool>(), None<bool>().Filter(Id));
        }

        [Test]
        public void FilterSomeFalse()
        {
            AssertEq(None<bool>(), Some(false).Filter(Id));
        }

        [Test]
        public void FilterSomeTrue()
        {
            AssertEq(Some(true), Some(true).Filter(Id));
        }

        [Test]
        public void DistOptionalOverNullableNone()
        {
            Assert.AreEqual(new Opt<int>?(), None<int?>().Dist());
        }

        [Test]
        public void DistOptionalOverNullableSomeNull()
        {
            Assert.AreEqual(new Opt<int>?(), Some<int?>(null).Dist());
        }

        [Test]
        public void DistOptionalOverNullableSome()
        {
            Assert.AreEqual(new Opt<int>?(Some(1)), Some<int?>(1).Dist());
        }

        [Test]
        public void DistNullableOverOptionalNull()
        {
            Assert.AreEqual(None<int?>(), new Opt<int>?().Dist());
        }

        [Test]
        public void DistNullableOverOptionalNone()
        {
            Assert.AreEqual(None<int?>(), new Opt<int>?(None<int>()).Dist());
        }

        [Test]
        public void DistNullableOverOptionalSome()
        {
            Assert.AreEqual(Some(new int?(1)), new Opt<int>?(Some(1)).Dist());
        }

        #region Usage

        [Test, Combinatorial]
        public void Linq(
            [Values(false, true)] bool hasX,
            [Values(false, true)] bool hasY,
            [Values(false, true)] bool hasZ,
            [Values(false, true)] bool passFilter)
        {
            var r =
                from x in Some(1).Filter(_ => hasX)
                from y in Some(2).Filter(_ => hasY)
                from z in Some(3).Filter(_ => hasZ)
                where passFilter
                select $"{x} {y} {z}";

            var e = hasX && hasY && hasZ && passFilter ? Some("1 2 3") : None<string>();

            Assert.AreEqual(e, r);
        }

        #endregion
    }
}
