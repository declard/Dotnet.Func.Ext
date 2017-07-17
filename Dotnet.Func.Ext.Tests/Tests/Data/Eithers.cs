using Dotnet.Func.Ext.Algebraic;
using Dotnet.Func.Ext.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static Dotnet.Func.Ext.Core.Functions;
using static Dotnet.Func.Ext.Data.Ctors;
using static Dotnet.Func.Ext.Data.Eithers;

namespace Dotnet.Func.Ext.Tests.Tests.Data
{
    [TestFixture]
    public class TEithers
    {
        private void AssertEq<left, right>(Either<left, right> l, Either<left, right> r) => Assert.AreEqual(l, r);

        [Test]
        public void Default()
        {
            AssertEq(Either<string>.Left(default(int)), default(Either<int, string>));
        }

        [Test]
        public void Pure()
        {
            var v = 1;
            AssertEq(Either<string>.Right(v), v.PureEither().With<string>());
        }
    }
}
