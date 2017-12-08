using Dotnet.Func.Ext.Data;
using NUnit.Framework;
using System;
using static Dotnet.Func.Ext.Core.Functions;
using static Dotnet.Func.Ext.Data.Ctors;

namespace Dotnet.Func.Ext.Tests.Data
{
    [TestFixture]
    public class TFunctions
    {
        private void AssertEq(Func<int, int> l, Func<int, int> r) => Assert.AreEqual(l(3), r(3));

        #region Laws

        // f |> map id = f
        [Test]
        public void FunctorId()
        {
            var f = Func<int, int>(x => x + 1);
            AssertEq(f.Map(Id), f);
        }

        // map (g.h) f = f |> map h |> map g
        [Test]
        public void FunctorComp()
        {
            var f = Func<int, int>(x => x + 1);
            var h = Func<int, int>(x => x + 3);
            var g = Func<int, int>(x => x * 3);
            AssertEq(f.Map(g.o(h)), f.Map(h).Map(g));
        }

        // f |> comap id = f
        [Test]
        public void CoFunctorId()
        {
            var f = Func<int, int>(x => x + 1);
            AssertEq(f.CoMap((int x) => x), f);
        }

        // comap (g.h) f = f |> comap g |> comap h
        [Test]
        public void CoFunctorComp()
        {
            var f = Func<int, int>(x => x + 1);
            var h = Func<int, int>(x => x + 3);
            var g = Func<int, int>(x => x * 3);
            AssertEq(f.CoMap(g.o(h)), f.CoMap(g).CoMap(h));
        }

        // pure v >>= f = f v
        [Test]
        public void MonadOutterId()
        {
            var f = Func<int, int>(x => x + 3).Map(Functions.Func.Pure<int, int>);
            var v = 1;
            AssertEq(v.PureFunc().With<int>().FlatMap(f), f(v));
        }

        // f >>= pure = f
        [Test]
        public void MonadInnerId()
        {
            var f = Func<int, int>(x => x + 1);
            AssertEq(f.FlatMap(Functions.Func.Pure<int, int>), f);
        }

        // f >>= h >>= g = f >>= \x -> h x >>= g
        [Test]
        public void MonadAssoc()
        {
            var f = Func<int, int>(x => x + 1);
            var g = Func<int, int>(x => x + 3).Map(Functions.Func.Pure<int, int>);
            var h = Func<int, int>(x => x * 3).Map(Functions.Func.Pure<int, int>);
            AssertEq(f.FlatMap(h).FlatMap(g), f.FlatMap(x => h(x).FlatMap(g)));
        }

        // pure id <*> v = v
        [Test]
        public void ApplicativeId()
        {
            var v = Func<int, int>(x => x + 1);
            AssertEq(Functions.Func.Pure<int, Func<int, int>>(Id).Ap(v), v);
        }

        // pure (.) <*> u <*> v <*> w = u <*> (v <*> w)
        [Test]
        public void ApplicativeComp()
        {
            var u = Curried<int, int, int>(x => y => x * y + 1);
            var v = Curried<int, int, int>(x => y => x * y + 2);
            var w = Func<int, int>(x => x + 3);
            AssertEq(Curry<Func<int, int>, Func<int, int>, Func<int, int>>(Compose).PureFunc().With<int>().Ap(u).Ap(v).Ap(w), u.Ap(v.Ap(w)));
        }

        // pure f <*> pure v = pure (f v)
        [Test]
        public void ApplicativeHomo()
        {
            var f = Func<int, int>(x => x + 1);
            var v = 2;
            AssertEq(f.PureFunc().With<int>().Ap(v.PureFunc().With<int>()), f(v).PureFunc().With<int>());
        }

        // u <*> pure v = pure ($ v) <*> u
        [Test]
        public void ApplicativeCommut()
        {
            var u = Curried<int, int, int>(x => y => x * y + 1);
            var v = 2;
            AssertEq(u.Ap(v.PureFunc().With<int>()), Func<Func<int, int>, int>(x => x(v)).PureFunc().With<int>().Ap(u));
        }

        #endregion

        #region Usage
        
        [Test]
        public void Map()
        {
            var f = Func<int, string>(x => $"{x}");
            var r = f.Map(s => $"'{s}'")(3);
            Assert.AreEqual("'3'", r);
        }

        [Test]
        public void CoMap()
        {
            var f = Func<int, string>(x => $"{x}");
            var r = f.CoMap((int x) => x + 1);
            Assert.AreEqual("3", r(2));
        }

        [Test]
        public void Flatten()
        {
            var f = Curried<int, int, string>(x => y => $"{x} {y}");
            var r = f.Flatten()(3);
            Assert.AreEqual("3 3", r);
        }

        [Test]
        public void Ap()
        {
            var f = Curried<int[], int, int, string>(x => y => z => $"{x[0]} {y} {z}").Ap(s => s[2]).Ap(s => s[1]);
            var r = f(new[] { 1, 2, 3 });
            Assert.AreEqual("1 3 2", r);
        }

        [Test]
        public void Linq()
        {
            var f =
                from x in Func<int[], int>(s => s[0])
                from y in Func<int[], int>(s => s[2])
                from z in Func<int[], int>(s => s[1])
                select $"{x} {y} {z}";

            var r = f(new[] { 1, 2, 3 });
            Assert.AreEqual("1 3 2", r);
        }

        #endregion
    }
}
