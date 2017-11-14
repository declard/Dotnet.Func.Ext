using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ListReadOnlyListAdapter<T> : IList<T>
    {
        public IReadOnlyList<T> InnerList { get; }

        public ListReadOnlyListAdapter(IReadOnlyList<T> e) { InnerList = e; }

        public T this[int index] { get => InnerList[index]; set => throw new InvalidOperationException(); }
        public int Count => InnerList.Count;
        public bool Contains(T item) => InnerList.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => InnerList.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();
        public int IndexOf(T item) => InnerList.IndexOf(item);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsReadOnly => true;

        public void Add(T item) => throw new InvalidOperationException();
        public void Clear() => throw new InvalidOperationException();
        public void Insert(int index, T item) => throw new InvalidOperationException();
        public bool Remove(T item) => throw new InvalidOperationException();
        public void RemoveAt(int index) => throw new InvalidOperationException();
    }
}
