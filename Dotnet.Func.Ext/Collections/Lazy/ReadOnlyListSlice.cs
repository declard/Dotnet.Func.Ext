using Dotnet.Func.Ext.Algebraic;
using Dotnet.Func.Ext.Data;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Dotnet.Func.Ext.Collections.Lazy
{
    public class ReadOnlyListSlice<valueˈ> : IReadOnlyList<valueˈ>
    {
        public IReadOnlyList<valueˈ> InnerList { get; }
        public int Offset { get; }

        public ReadOnlyListSlice(IReadOnlyList<valueˈ> list, int offset, int length)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (offset < 0 || offset >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (length < 0 || offset + length > list.Count)
                throw new ArgumentOutOfRangeException(nameof(length));

            InnerList = list;
            Offset = offset;
            Count = length;
        }

        public valueˈ this[int index] => index.IsOnSemiInterval(index, index + Count, AInt32.Class)
            ? InnerList[index + Offset]
            : throw new IndexOutOfRangeException();

        public int Count { get; }

        public IEnumerator<valueˈ> GetEnumerator()
        {
            for (var i = Offset; i < Count; i++)
                yield return InnerList[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
