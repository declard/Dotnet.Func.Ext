using NUnit.Framework;
using static Dotnet.Func.Ext.Data.Continuations;
using Dotnet.Func.Ext.Data;
using static Dotnet.Func.Ext.Functions;
using static Dotnet.Func.Ext.Data.Units;
using System;

namespace Dotnet.Func.Ext.Tests.Data
{
    [TestFixture]
    public class Continuations
    {
        private void AssertEq<a>(Cont<a, a> l, Cont<a, a> r) => Assert.AreEqual(l.Uncont(), r.Uncont());

        [Test]
        public void Lazy()
        {
            var e = false;
            var k = Ctors.Cont<string, int>(kk => { e = true; return kk(1); });
            Assert.IsFalse(e);
            var r = k.Case(i => i.ToString());
            Assert.AreEqual("1", r);
            Assert.IsTrue(e);
        }

        [Test]
        public void FunctorId()
        {
            var k = Ctors.Cont<int, int>(kk => kk(1));
            AssertEq(k.Map(Id), k);
        }

        [Test]
        public void FunctorComp()
        {
            var k = Ctors.Cont<int, int>(kk => kk(1));
            var f = Func<int, int>(x => x + 3);
            var g = Func<int, int>(x => x * 3);
            AssertEq(k.Map(g.o(f)), k.Map(f).Map(g));
        }

        [Test]
        public void MonadOutterId()
        {
            var f = Func<int, int>(x => x + 3).Map(Cont.Pure<int, int>);
            AssertEq(1.PureCont().With<int>().Bind(f), f(1));
        }

        [Test]
        public void MonadInnerId()
        {
            var k = 1.PureCont().With<int>();
            AssertEq(k.Bind(Cont.Pure<int, int>), k);
        }

        [Test]
        public void MonadAssoc()
        {
            var k = 1.PureCont().With<int>();
            var f = Func<int, int>(x => x + 3).Map(Cont.Pure<int, int>);
            var g = Func<int, int>(x => x * 3).Map(Cont.Pure<int, int>);
            AssertEq(k.Bind(f).Bind(g), k.Bind(kk => f(kk).Bind(g)));
        }

        [Test]
        public void LinqTest()
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
        public void CallCcTest()
        {
            var validateName = Func<string, Func<string, Cont<string, Unit>>, Cont<string, Unit>> ((name, exit) =>
                name == "" ? exit("You forgot to tell me your name!") : Ctors.Unit().PureCont().With<string>());

            var whatsYourName = Func<string, string>(name => CallCC<string, string, Unit>(exit =>
                validateName(name, exit).Bind(_ =>
                $"Welcome, {name}!".PureCont().With<string>())).Uncont());

            Assert.AreEqual(whatsYourName("Mr. Callback"), "Welcome, Mr. Callback!");
            Assert.AreEqual(whatsYourName(""), "You forgot to tell me your name!");
        }
        
    }
}
