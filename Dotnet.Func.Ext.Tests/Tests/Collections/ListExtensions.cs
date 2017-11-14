using Dotnet.Func.Ext.Collections;
using Dotnet.Func.Ext.Collections.Casts;
using Dotnet.Func.Ext.Data;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using static Dotnet.Func.Ext.Data.Ctors;

namespace Dotnet.Func.Ext.Tests.Tests.Collections
{
    [TestFixture]
    public class TListExtensions
    {
        [Test]
        public void BinarySearchNullList()
        {
            IReadOnlyList<int> list = null;
            Assert.Throws<ArgumentNullException>(() => list.BinarySearch(0, 1, 42));
        }

        [Test]
        public void BinarySearchIndexTooSmall()
        {
            var list = new List<int> { 42 }.AsReadOnlyList();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.BinarySearch(-1, 1, 42));
        }

        [Test]
        public void BinarySearchIndexTooBig()
        {
            var list = new List<int> { 42 }.AsReadOnlyList();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.BinarySearch(1, 1, 42));
        }

        [Test]
        public void BinarySearchLengthTooSmall()
        {
            var list = new List<int> { 42 }.AsReadOnlyList();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.BinarySearch(0, -1, 42));
        }

        [Test]
        public void BinarySearchEmpty()
        {
            var list = new List<int> { 42 }.AsReadOnlyList();
            var result = list.BinarySearch(0, 0, 42);
            Assert.AreEqual(None<int>(), result);
        }

        [Test]
        public void BinarySearchLengthTooBig()
        {
            var list = new List<int> { 42 }.AsReadOnlyList();
            Assert.Throws<ArgumentOutOfRangeException>(() => list.BinarySearch(0, 2, 42));
        }

        [Test]
        public void BinarySearchExactMatch()
        {
            var list = new List<int> { 0, 1, 42, 83, 84 }.AsReadOnlyList();
            var result = list.BinarySearch(0, 5, 42);
            Assert.AreEqual(2.PureOpt(), result);
        }

        [Test]
        public void BinarySearchExactNoMatchIndex()
        {
            var list = new List<int> { 0, 1, 42, 83, 84 }.AsReadOnlyList();
            var result = list.BinarySearch(3, 2, 42);
            Assert.AreEqual(None<int>(), result);
        }

        [Test]
        public void BinarySearchExactNoMatchLength()
        {
            var list = new List<int> { 0, 1, 42, 83, 84 }.AsReadOnlyList();
            var result = list.BinarySearch(0, 2, 42);
            Assert.AreEqual(None<int>(), result);
        }

        [Test]
        public void BinarySearchExactNoMatchValue()
        {
            var list = new List<int> { 0, 1, 83, 84 }.AsReadOnlyList();
            var result = list.BinarySearch(0, 4, 42);
            Assert.AreEqual(None<int>(), result);
        }

        [Test]
        public void BinarySearchExactFallbackDown()
        {
            var list = new List<int> { 0, 1, 83, 84 }.AsReadOnlyList();
            var result = list.BinarySearch(0, 4, 42, fallback: Lt());
            Assert.AreEqual(1.PureOpt(), result);
        }

        [Test]
        public void BinarySearchExactFallbackUp()
        {
            var list = new List<int> { 0, 1, 83, 84 }.AsReadOnlyList();
            var result = list.BinarySearch(0, 4, 42, fallback: Gt());
            Assert.AreEqual(2.PureOpt(), result);
        }

        [Test]
        public void BinarySearchExactFallbackDownBelow()
        {
            var list = new List<int> { 42, 43, 44 }.AsReadOnlyList();
            var result = list.BinarySearch(1, 2, 42, fallback: Lt());
            Assert.AreEqual(None<int>(), result);
        }

        [Test]
        public void BinarySearchExactFallbackUpAbove()
        {
            var list = new List<int> { 40, 41, 42 }.AsReadOnlyList();
            var result = list.BinarySearch(0, 2, 42, fallback: Gt());
            Assert.AreEqual(None<int>(), result);
        }
    }
}
