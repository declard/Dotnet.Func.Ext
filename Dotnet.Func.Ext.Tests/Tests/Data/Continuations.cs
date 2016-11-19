using NUnit.Framework;
using static Dotnet.Func.Ext.Data.Continuations;
using Dotnet.Func.Ext.Data;
using static Dotnet.Func.Ext.Data.Functions;
using static Dotnet.Func.Ext.Core.Functions;
using static Dotnet.Func.Ext.Data.Units;
using static Dotnet.Func.Ext.Data.Ctors;
using static Dotnet.Func.Ext.Data.Continuations.Cont;
using System;

namespace Dotnet.Func.Ext.Tests.Data
{
    [TestFixture]
    public class TContinuations
    {
        private void AssertEq<a>(Cont<a, a> l, Cont<a, a> r) => Assert.AreEqual(l.Uncont(), r.Uncont());

        #region Laws

        // f |> map id = f
        [Test]
        public void FunctorId()
        {
            var f = Cont<int, int>(kk => kk(1));
            AssertEq(f.Map(Id), f);
        }

        // map (g.h) f = f |> map h |> map g
        [Test]
        public void FunctorComposition()
        {
            var f = Cont<int, int>(kk => kk(1));
            var h = Func<int, int>(x => x + 3);
            var g = Func<int, int>(x => x * 3);
            AssertEq(f.Map(g.o(h)), f.Map(h).Map(g));
        }

        // pure v >>= f = f v
        [Test]
        public void MonadOutterId()
        {
            var f = Func<int, int>(x => x + 3).Map(Pure<int, int>);
            var v = 1;
            AssertEq(v.PureCont().With<int>().Bind(f), f(v));
        }

        // f >>= id = f
        [Test]
        public void MonadInnerId()
        {
            var f = 1.PureCont().With<int>();
            AssertEq(f.Bind(Pure<int, int>), f);
        }

        // f >>= h >>= g = f >>= \x -> h x >>= g
        [Test]
        public void MonadAssoc()
        {
            var f = 1.PureCont().With<int>();
            var h = Func<int, int>(x => x + 3).Map(Pure<int, int>);
            var g = Func<int, int>(x => x * 3).Map(Pure<int, int>);
            AssertEq(f.Bind(h).Bind(g), f.Bind(kk => h(kk).Bind(g)));
        }

        // pure id <*> v = v
        [Test]
        public void ApplicativeId()
        {
            var v = 1.PureCont().With<int>();
            AssertEq(Pure<int, Func<int, int>>(Id).Ap(v), v);
        }

        // pure (.) <*> u <*> v <*> w = u <*> (v <*> w)
        [Test]
        public void ApplicativeComp()
        {
            var u = Cont<int, Func<int, int>>(x => x(y => y + 1));
            var v = Cont<int, Func<int, int>>(x => x(y => y + 2));
            var w = Cont<int, int>(k => k(3));
            AssertEq(Curry<Func<int, int>, Func<int, int>, Func<int, int>>(Compose).PureCont().With<int>().Ap(u).Ap(v).Ap(w), u.Ap(v.Ap(w)));
        }

        // pure f <*> pure v = pure (f v)
        [Test]
        public void ApplicativeHomo()
        {
            var f = Func<int, int>(x => x + 1);
            var v = 2;
            AssertEq(f.PureCont().With<int>().Ap(v.PureCont().With<int>()), f(v).PureCont().With<int>());
        }

        // u <*> pure v = pure ($ v) <*> u
        [Test]
        public void ApplicativeCommut()
        {
            var u = Cont<int, Func<int, int>>(x => x(y => y + 1));
            var v = 2;
            AssertEq(u.Ap(v.PureCont().With<int>()), Func<Func<int, int>, int>(x => x(v)).PureCont().With<int>().Ap(u));
        }

        #endregion

        #region Usage

        [Test]
        public void Laziness()
        {
            var e = false;
            var k = Cont<string, int>(kk => { e = true; return kk(1); });
            Assert.IsFalse(e);
            var r = k.Case(i => i.ToString());
            Assert.AreEqual("1", r);
            Assert.IsTrue(e);
        }

        [Test]
        public void Linq()
        {
            var f = Func<int, int, int, int>((x, y, z) => x + y * z);

            var q =
                from x in 2.PureCont().With<int>()
                from y in 3.PureCont().With<int>()
                from z in 4.PureCont().With<int>()
                select f(x, y, z);

            Assert.AreEqual(q.Uncont(), 14);
        }

        /*
        https://hackage.haskell.org/package/mtl-2.2.1/docs/Control-Monad-Cont.html

        validateName name exit = do
          when (null name) (exit "You forgot to tell me your name!")

        whatsYourName :: String -> String
        whatsYourName name =
          (`runCont` id) $ do
            response <- callCC $ \exit -> do
              validateName name exit
              return $ "Welcome, " ++ name ++ "!"
            return response
        */
        [Test]
        public void CallCc()
        {
            var validateName = Func<string, Func<string, Cont<string, Unit>>, Cont<string, Unit>>((name, exit) =>
               name == "" ? exit("You forgot to tell me your name!") : ContUnit<string>(Ctors.Unit()));

            var whatsYourName = Func<string, string>(name => Run(Id,
                from response in CallCC<string, string, Unit>(exit =>
                    from _ in validateName(name, exit)
                    select $"Welcome, {name}!")
                select response));

            Assert.AreEqual(whatsYourName("Mr. Callback"), "Welcome, Mr. Callback!");
            Assert.AreEqual(whatsYourName(""), "You forgot to tell me your name!");
        }

        #endregion
        
    }
}
