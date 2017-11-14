using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class CollectionReadOnlyCollectionAdapter<T> : ICollection<T>
    {
        public IReadOnlyCollection<T> InnerCollection { get; }

        public CollectionReadOnlyCollectionAdapter(IReadOnlyCollection<T> e) { InnerCollection = e; }

        public int Count => InnerCollection.Count;
        public bool Contains(T item) => InnerCollection.Contains(item);
        public void CopyTo(T[] array, int arrayIndex) => InnerCollection.CopyTo(array, arrayIndex);
        public IEnumerator<T> GetEnumerator() => InnerCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsReadOnly => true;

        public void Add(T item) => throw new InvalidOperationException();
        public void Clear() => throw new InvalidOperationException();
        public bool Remove(T item) => throw new InvalidOperationException();
    }
}
