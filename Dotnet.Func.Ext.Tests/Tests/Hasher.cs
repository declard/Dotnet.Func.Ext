namespace Dotnet.Func.Ext.Tests.Tests
{
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class THasher
    {
        [Test]
        public void EmptyHash()
        {
            var hasher = new Hasher { };

            Assert.AreEqual(null, hasher.Hash);
            Assert.AreEqual(17, hasher.GetHashCode());
        }

        [Test]
        public void NullsHash()
        {
            var hasher = new Hasher
            {
                (string)null,
                (int?)null,
                (DateTime?)null,
            };

            var actual = hasher.Hash;
            var expected = new Hasher()
                .Combine(null)
                .Combine(null)
                .Combine(null)
                .GetHashCode();

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void MixedHash()
        {
            var str = "asd";
            var flt = 1.5;
            var dcm = 12m;
            var ntgr = 3;
            var chr = 'v';
            var obj = (object)null;
            var dt = new DateTime(2025, 03, 02);
            var nflt = (float?)null;

            var hasher = new Hasher
            {
                str,
                flt,
                dcm,
                ntgr,
                chr,
                obj,
                dt,
                nflt,
            };

            var actual = hasher.Hash;

            var expected = new Hasher()
                .Combine(str.GetHashCode())
                .Combine(flt.GetHashCode())
                .Combine(dcm.GetHashCode())
                .Combine(ntgr.GetHashCode())
                .Combine(chr.GetHashCode())
                .Combine(obj?.GetHashCode())
                .Combine(dt.GetHashCode())
                .Combine(nflt?.GetHashCode());

            Assert.AreEqual(expected.GetHashCode(), actual);
        }
    }
}
