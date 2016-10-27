using NUnit.Framework;
using System.Linq;

namespace Dotnet.Func.Ext.Tests.Collections
{
    [TestFixture]
    public class Ranges
    {
        private static int[] Inf(int v) => Enumerable.Repeat(v, 10).ToArray();

        [Test, TestCaseSource(nameof(EnumFromByToCases))]
        public void EnumFromByTo(int from, int by, int to, int[] expected)
        {
            var actual = Ext.Collections.Ranges.EnumFromByTo(from, by, to, Algebras.AInt32.Class).Take(10).ToArray();

            Assert.AreEqual(expected, actual);
        }
        
        static object[] EnumFromByToCases =
        {
            new object[] { -2, -2, -2, new int[] { -2 } },
            new object[] { -2, -2, -1, new int[] { } },
            new object[] { -2, -2, 0, new int[] { } },
            new object[] { -2, -2, +1, new int[] { } },
            new object[] { -2, -2, +2, new int[] { } },
            new object[] { -2, -1, -2, new int[] { -2 } },
            new object[] { -2, -1, -1, new int[] { } },
            new object[] { -2, -1, 0, new int[] { } },
            new object[] { -2, -1, +1, new int[] { } },
            new object[] { -2, -1, +2, new int[] { } },
            new object[] { -2, 0, -2, Inf(-2) },
            new object[] { -2, 0, -1, Inf(-2) },
            new object[] { -2, 0, 0, Inf(-2) },
            new object[] { -2, 0, +1, Inf(-2) },
            new object[] { -2, 0, +2, Inf(-2) },
            new object[] { -2, +1, -2, new int[] { -2 } },
            new object[] { -2, +1, -1, new int[] { -2, -1 } },
            new object[] { -2, +1, 0, new int[] { -2, -1, 0 } },
            new object[] { -2, +1, +1, new int[] { -2, -1, 0, +1 } },
            new object[] { -2, +1, +2, new int[] { -2, -1, 0, +1, +2 } },
            new object[] { -2, +2, -2, new int[] { -2 } },
            new object[] { -2, +2, -1, new int[] { -2 } },
            new object[] { -2, +2, 0, new int[] { -2, 0 } },
            new object[] { -2, +2, +1, new int[] { -2, 0 } },
            new object[] { -2, +2, +2, new int[] { -2, 0, +2 } },

            new object[] { -1, -2, -2, new int[] { -1 } },
            new object[] { -1, -2, -1, new int[] { -1 } },
            new object[] { -1, -2, 0, new int[] { } },
            new object[] { -1, -2, +1, new int[] { } },
            new object[] { -1, -2, +2, new int[] { } },
            new object[] { -1, -1, -2, new int[] { -1, -2 } },
            new object[] { -1, -1, -1, new int[] { -1 } },
            new object[] { -1, -1, 0, new int[] { } },
            new object[] { -1, -1, +1, new int[] { } },
            new object[] { -1, -1, +2, new int[] { } },
            new object[] { -1, 0, -2, Inf(-1) },
            new object[] { -1, 0, -1, Inf(-1) },
            new object[] { -1, 0, 0, Inf(-1) },
            new object[] { -1, 0, +1, Inf(-1) },
            new object[] { -1, 0, +2, Inf(-1) },
            new object[] { -1, +1, -2, new int[] { } },
            new object[] { -1, +1, -1, new int[] { -1 } },
            new object[] { -1, +1, 0, new int[] { -1, 0 } },
            new object[] { -1, +1, +1, new int[] { -1, 0, +1 } },
            new object[] { -1, +1, +2, new int[] { -1, 0, +1, +2 } },
            new object[] { -1, +2, -2, new int[] { } },
            new object[] { -1, +2, -1, new int[] { -1 } },
            new object[] { -1, +2, 0, new int[] { -1 } },
            new object[] { -1, +2, +1, new int[] { -1, +1 } },
            new object[] { -1, +2, +2, new int[] { -1, +1 } },

            new object[] { 0, -2, -2, new int[] { 0, -2 } },
            new object[] { 0, -2, -1, new int[] { 0 } },
            new object[] { 0, -2, 0, new int[] { 0 } },
            new object[] { 0, -2, +1, new int[] { } },
            new object[] { 0, -2, +2, new int[] { } },
            new object[] { 0, -1, -2, new int[] { 0, -1, -2 } },
            new object[] { 0, -1, -1, new int[] { 0, -1 } },
            new object[] { 0, -1, 0, new int[] { 0 } },
            new object[] { 0, -1, +1, new int[] { } },
            new object[] { 0, -1, +2, new int[] { } },
            new object[] { 0, 0, -2, Inf(0) },
            new object[] { 0, 0, -1, Inf(0) },
            new object[] { 0, 0, 0, Inf(0) },
            new object[] { 0, 0, +1, Inf(0) },
            new object[] { 0, 0, +2, Inf(0) },
            new object[] { 0, +1, -2, new int[] { } },
            new object[] { 0, +1, -1, new int[] { } },
            new object[] { 0, +1, 0, new int[] { 0 } },
            new object[] { 0, +1, +1, new int[] { 0, +1 } },
            new object[] { 0, +1, +2, new int[] { 0, +1, +2 } },
            new object[] { 0, +2, -2, new int[] { } },
            new object[] { 0, +2, -1, new int[] { } },
            new object[] { 0, +2, 0, new int[] { 0 } },
            new object[] { 0, +2, +1, new int[] { 0 } },
            new object[] { 0, +2, +2, new int[] { 0, +2 } },

            new object[] { +1, -2, -2, new int[] { +1, -1 } },
            new object[] { +1, -2, -1, new int[] { +1, -1 } },
            new object[] { +1, -2, 0, new int[] { +1 } },
            new object[] { +1, -2, +1, new int[] { +1 } },
            new object[] { +1, -2, +2, new int[] { } },
            new object[] { +1, -1, -2, new int[] { +1, 0, -1, -2 } },
            new object[] { +1, -1, -1, new int[] { +1, 0, -1 } },
            new object[] { +1, -1, 0, new int[] { +1, 0 } },
            new object[] { +1, -1, +1, new int[] { +1 } },
            new object[] { +1, -1, +2, new int[] { } },
            new object[] { +1, 0, -2, Inf(+1) },
            new object[] { +1, 0, -1, Inf(+1) },
            new object[] { +1, 0, 0, Inf(+1) },
            new object[] { +1, 0, +1, Inf(+1) },
            new object[] { +1, 0, +2, Inf(+1) },
            new object[] { +1, +1, -2, new int[] { } },
            new object[] { +1, +1, -1, new int[] { } },
            new object[] { +1, +1, 0, new int[] { } },
            new object[] { +1, +1, +1, new int[] { +1 } },
            new object[] { +1, +1, +2, new int[] { +1, +2 } },
            new object[] { +1, +2, -2, new int[] { } },
            new object[] { +1, +2, -1, new int[] { } },
            new object[] { +1, +2, 0, new int[] { } },
            new object[] { +1, +2, +1, new int[] { +1 } },
            new object[] { +1, +2, +2, new int[] { +1 } },

            new object[] { +2, -2, -2, new int[] { +2, 0, -2 } },
            new object[] { +2, -2, -1, new int[] { +2, 0 } },
            new object[] { +2, -2, 0, new int[] { +2, 0 } },
            new object[] { +2, -2, +1, new int[] { +2 } },
            new object[] { +2, -2, +2, new int[] { +2 } },
            new object[] { +2, -1, -2, new int[] { +2, +1, 0, -1, -2 } },
            new object[] { +2, -1, -1, new int[] { +2, +1, 0, -1 } },
            new object[] { +2, -1, 0, new int[] { +2, +1, 0 } },
            new object[] { +2, -1, +1, new int[] { +2, +1 } },
            new object[] { +2, -1, +2, new int[] { +2 } },
            new object[] { +2, 0, -2, Inf(+2) },
            new object[] { +2, 0, -1, Inf(+2) },
            new object[] { +2, 0, 0, Inf(+2) },
            new object[] { +2, 0, +1, Inf(+2) },
            new object[] { +2, 0, +2, Inf(+2) },
            new object[] { +2, +1, -2, new int[] { } },
            new object[] { +2, +1, -1, new int[] { } },
            new object[] { +2, +1, 0, new int[] { } },
            new object[] { +2, +1, +1, new int[] { } },
            new object[] { +2, +1, +2, new int[] { +2 } },
            new object[] { +2, +2, -2, new int[] { } },
            new object[] { +2, +2, -1, new int[] { } },
            new object[] { +2, +2, 0, new int[] { } },
            new object[] { +2, +2, +1, new int[] { } },
            new object[] { +2, +2, +2, new int[] { +2 } },
        };


        [Test, TestCaseSource(nameof(EnumFromThenToCases))]
        public void EnumFromThenTo(int from, int then, int to, int[] expected)
        {
            var actual = Ext.Collections.Ranges.EnumFromThenTo(from, then, to, Algebras.AInt32.Class).Take(10).ToArray();

            Assert.AreEqual(expected, actual);
        }

        static object[] EnumFromThenToCases =
        {
            new object[] { -2, -2, -2, Inf(-2) },
            new object[] { -2, -2, -1, Inf(-2) },
            new object[] { -2, -2, 0, Inf(-2) },
            new object[] { -2, -2, +1, Inf(-2) },
            new object[] { -2, -2, +2, Inf(-2) },
            new object[] { -2, -1, -2, new int[] { -2 } },
            new object[] { -2, -1, -1, new int[] { -2, -1 } },
            new object[] { -2, -1, 0, new int[] { -2, -1, 0 } },
            new object[] { -2, -1, +1, new int[] { -2, -1, 0, +1 } },
            new object[] { -2, -1, +2, new int[] { -2, -1, 0, +1, +2 } },
            new object[] { -2, 0, -2, new int[] { -2 } },
            new object[] { -2, 0, -1, new int[] { -2 } },
            new object[] { -2, 0, 0, new int[] { -2, 0 } },
            new object[] { -2, 0, +1, new int[] { -2, 0 } },
            new object[] { -2, 0, +2, new int[] { -2, 0, +2 } },
            new object[] { -2, +1, -2, new int[] { -2 } },
            new object[] { -2, +1, -1, new int[] { -2 } },
            new object[] { -2, +1, 0, new int[] { -2 } },
            new object[] { -2, +1, +1, new int[] { -2, +1 } },
            new object[] { -2, +1, +2, new int[] { -2, +1 } },
            new object[] { -2, +2, -2, new int[] { -2 } },
            new object[] { -2, +2, -1, new int[] { -2 } },
            new object[] { -2, +2, 0, new int[] { -2 } },
            new object[] { -2, +2, +1, new int[] { -2 } },
            new object[] { -2, +2, +2, new int[] { -2, +2 } },

            new object[] { -1, -2, -2, new int[] { -1, -2 } },
            new object[] { -1, -2, -1, new int[] { -1 } },
            new object[] { -1, -2, 0, new int[] { } },
            new object[] { -1, -2, +1, new int[] { } },
            new object[] { -1, -2, +2, new int[] { } },
            new object[] { -1, -1, -2, Inf(-1) },
            new object[] { -1, -1, -1, Inf(-1) },
            new object[] { -1, -1, 0, Inf(-1) },
            new object[] { -1, -1, +1, Inf(-1) },
            new object[] { -1, -1, +2, Inf(-1) },
            new object[] { -1, 0, -2, new int[] { } },
            new object[] { -1, 0, -1, new int[] { -1 } },
            new object[] { -1, 0, 0, new int[] { -1, 0 } },
            new object[] { -1, 0, +1, new int[] { -1, 0, +1 } },
            new object[] { -1, 0, +2, new int[] { -1, 0, +1, +2 } },
            new object[] { -1, +1, -2, new int[] { } },
            new object[] { -1, +1, -1, new int[] { -1 } },
            new object[] { -1, +1, 0, new int[] { -1 } },
            new object[] { -1, +1, +1, new int[] { -1, +1 } },
            new object[] { -1, +1, +2, new int[] { -1, +1 } },
            new object[] { -1, +2, -2, new int[] { } },
            new object[] { -1, +2, -1, new int[] { -1 } },
            new object[] { -1, +2, 0, new int[] { -1 } },
            new object[] { -1, +2, +1, new int[] { -1 } },
            new object[] { -1, +2, +2, new int[] { -1, +2 } },

            new object[] { 0, -2, -2, new int[] { 0, -2 } },
            new object[] { 0, -2, -1, new int[] { 0 } },
            new object[] { 0, -2, 0, new int[] { 0 } },
            new object[] { 0, -2, +1, new int[] { } },
            new object[] { 0, -2, +2, new int[] { } },
            new object[] { 0, -1, -2, new int[] { 0, -1, -2 } },
            new object[] { 0, -1, -1, new int[] { 0, -1 } },
            new object[] { 0, -1, 0, new int[] { 0 } },
            new object[] { 0, -1, +1, new int[] { } },
            new object[] { 0, -1, +2, new int[] { } },
            new object[] { 0, 0, -2, Inf(0) },
            new object[] { 0, 0, -1, Inf(0) },
            new object[] { 0, 0, 0, Inf(0) },
            new object[] { 0, 0, +1, Inf(0) },
            new object[] { 0, 0, +2, Inf(0) },
            new object[] { 0, +1, -2, new int[] { } },
            new object[] { 0, +1, -1, new int[] { } },
            new object[] { 0, +1, 0, new int[] { 0 } },
            new object[] { 0, +1, +1, new int[] { 0, +1 } },
            new object[] { 0, +1, +2, new int[] { 0, +1, +2 } },
            new object[] { 0, +2, -2, new int[] { } },
            new object[] { 0, +2, -1, new int[] { } },
            new object[] { 0, +2, 0, new int[] { 0 } },
            new object[] { 0, +2, +1, new int[] { 0 } },
            new object[] { 0, +2, +2, new int[] { 0, +2 } },

            new object[] { +1, -2, -2, new int[] { +1, -2 } },
            new object[] { +1, -2, -1, new int[] { +1 } },
            new object[] { +1, -2, 0, new int[] { +1 } },
            new object[] { +1, -2, +1, new int[] { +1 } },
            new object[] { +1, -2, +2, new int[] { } },
            new object[] { +1, -1, -2, new int[] { +1, -1 } },
            new object[] { +1, -1, -1, new int[] { +1, -1 } },
            new object[] { +1, -1, 0, new int[] { +1 } },
            new object[] { +1, -1, +1, new int[] { +1 } },
            new object[] { +1, -1, +2, new int[] { } },
            new object[] { +1, 0, -2, new int[] { +1, 0, -1, -2 } },
            new object[] { +1, 0, -1, new int[] { +1, 0, -1 } },
            new object[] { +1, 0, 0, new int[] { +1, 0 } },
            new object[] { +1, 0, +1, new int[] { +1 } },
            new object[] { +1, 0, +2, new int[] { } },
            new object[] { +1, +1, -2, Inf(+1) },
            new object[] { +1, +1, -1, Inf(+1) },
            new object[] { +1, +1, 0, Inf(+1) },
            new object[] { +1, +1, +1, Inf(+1) },
            new object[] { +1, +1, +2, Inf(+1) },
            new object[] { +1, +2, -2, new int[] { } },
            new object[] { +1, +2, -1, new int[] { } },
            new object[] { +1, +2, 0, new int[] { } },
            new object[] { +1, +2, +1, new int[] { +1 } },
            new object[] { +1, +2, +2, new int[] { +1, +2 } },

            new object[] { +2, -2, -2, new int[] { +2, -2 } },
            new object[] { +2, -2, -1, new int[] { +2 } },
            new object[] { +2, -2, 0, new int[] { +2 } },
            new object[] { +2, -2, +1, new int[] { +2 } },
            new object[] { +2, -2, +2, new int[] { +2 } },
            new object[] { +2, -1, -2, new int[] { +2, -1 } },
            new object[] { +2, -1, -1, new int[] { +2, -1 } },
            new object[] { +2, -1, 0, new int[] { +2 } },
            new object[] { +2, -1, +1, new int[] { +2 } },
            new object[] { +2, -1, +2, new int[] { +2 } },
            new object[] { +2, 0, -2, new int[] { +2, 0, -2 } },
            new object[] { +2, 0, -1, new int[] { +2, 0 } },
            new object[] { +2, 0, 0, new int[] { +2, 0 } },
            new object[] { +2, 0, +1, new int[] { +2 } },
            new object[] { +2, 0, +2, new int[] { +2 } },
            new object[] { +2, +1, -2, new int[] { +2, +1, 0, -1, -2 } },
            new object[] { +2, +1, -1, new int[] { +2, +1, 0, -1 } },
            new object[] { +2, +1, 0, new int[] { +2, +1, 0 } },
            new object[] { +2, +1, +1, new int[] { +2, +1 } },
            new object[] { +2, +1, +2, new int[] { +2 } },
            new object[] { +2, +2, -2, Inf(+2) },
            new object[] { +2, +2, -1, Inf(+2) },
            new object[] { +2, +2, 0, Inf(+2) },
            new object[] { +2, +2, +1, Inf(+2) },
            new object[] { +2, +2, +2, Inf(+2) },
        };

        [Test, TestCaseSource(nameof(EnumFromByCases))]
        public void EnumFromBy(int from, int by, int[] expected)
        {
            var actual = Ext.Collections.Ranges.EnumFromBy(from, by, Algebras.AInt32.Class).Take(5).ToArray();

            Assert.AreEqual(expected, actual);
        }

        static object[] EnumFromByCases =
        {
            new object[] { -2, -2, new int[] { -2, -4, -6, -8, -10 } },
            new object[] { -2, -1, new int[] { -2, -3, -4, -5, -6 } },
            new object[] { -2, 0, new int[] { -2, -2, -2, -2, -2 } },
            new object[] { -2, +1, new int[] { -2, -1, 0, +1, +2 } },
            new object[] { -2, +2, new int[] { -2, 0, +2, +4, +6 } },

            new object[] { 0, -2, new int[] { 0, -2, -4, -6, -8 } },
            new object[] { 0, -1, new int[] { 0, -1, -2, -3, -4 } },
            new object[] { 0, 0, new int[] { 0, 0, 0, 0, 0 } },
            new object[] { 0, +1, new int[] { 0, +1, +2, +3, +4 } },
            new object[] { 0, +2, new int[] { 0, +2, +4, +6, +8 } },

            new object[] { +1, -2, new int[] { +1, -1, -3, -5, -7 } },
            new object[] { +1, -1, new int[] { +1, 0, -1, -2, -3 } },
            new object[] { +1, 0, new int[] { +1, +1, +1, +1, +1 } },
            new object[] { +1, +1, new int[] { +1, +2, +3, +4, +5 } },
            new object[] { +1, +2, new int[] { +1, +3, +5, +7, +9 } },
        };

        [Test, TestCaseSource(nameof(EnumFromThenCases))]
        public void EnumFromThen(int from, int then, int[] expected)
        {
            var actual = Ext.Collections.Ranges.EnumFromThen(from, then, Algebras.AInt32.Class).Take(5).ToArray();

            Assert.AreEqual(expected, actual);
        }

        static object[] EnumFromThenCases =
        {
            new object[] { -2, -2, new int[] { -2, -2, -2, -2, -2 } },
            new object[] { -2, -1, new int[] { -2, -1, 0, +1, +2 } },
            new object[] { -2, 0, new int[] { -2, 0, +2, +4, +6 } },
            new object[] { -2, +1, new int[] { -2, +1, +4, +7, +10 } },
            new object[] { -2, +2, new int[] { -2, 2, 6, 10, 14 } },

            new object[] { 0, -2, new int[] { 0, -2, -4, -6, -8 } },
            new object[] { 0, -1, new int[] { 0, -1, -2, -3, -4 } },
            new object[] { 0, 0, new int[] { 0, 0, 0, 0, 0 } },
            new object[] { 0, +1, new int[] { 0, +1, +2, +3, +4 } },
            new object[] { 0, +2, new int[] { 0, +2, +4, +6, +8 } },

            new object[] { +1, -2, new int[] { +1, -2, -5, -8, -11 } },
            new object[] { +1, -1, new int[] { +1, -1, -3, -5, -7 } },
            new object[] { +1, 0, new int[] { +1, 0, -1, -2, -3 } },
            new object[] { +1, +1, new int[] { +1, +1, +1, +1, +1 } },
            new object[] { +1, +2, new int[] { +1, +2, +3, +4, +5 } },
        };

        [Test, TestCaseSource(nameof(EnumFromCases))]
        public void EnumFrom(int from, int[] expected)
        {
            var actual = Ext.Collections.Ranges.EnumFrom(from, Algebras.AInt32.Class).Take(5).ToArray();

            Assert.AreEqual(expected, actual);
        }

        static object[] EnumFromCases =
        {
            new object[] { -2, new int[] { -2, -1, 0, +1, +2 } },
            new object[] { 0, new int[] { 0, +1, +2, +3, +4 } },
            new object[] { +1, new int[] { +1, +2, +3, +4, +5 } },
        };

        [Test, TestCaseSource(nameof(EnumFromToCases))]
        public void EnumFromTo(int from, int to, int[] expected)
        {
            var actual = Ext.Collections.Ranges.EnumFromTo(from, to, Algebras.AInt32.Class).ToArray();

            Assert.AreEqual(expected, actual);
        }

        static object[] EnumFromToCases =
        {
            new object[] { -2, -2, new int[] { -2 } },
            new object[] { -2, -1, new int[] { -2, -1 } },
            new object[] { -2, 0, new int[] { -2, -1, 0 } },
            new object[] { -2, +1, new int[] { -2, -1, 0, +1 } },
            new object[] { -2, +2, new int[] { -2, -1, 0, +1, +2} },

            new object[] { 0, -2, new int[] { 0, -1, -2} },
            new object[] { 0, -1, new int[] { 0, -1 } },
            new object[] { 0, 0, new int[] { 0 } },
            new object[] { 0, +1, new int[] { 0, +1 } },
            new object[] { 0, +2, new int[] { 0, +1, +2 } },

            new object[] { +1, -2, new int[] { +1, 0, -1, -2 } },
            new object[] { +1, -1, new int[] { +1, 0, -1 } },
            new object[] { +1, 0, new int[] { +1, 0 } },
            new object[] { +1, +1, new int[] { +1 } },
            new object[] { +1, +2, new int[] { +1, +2 } },
        };
    }
}
