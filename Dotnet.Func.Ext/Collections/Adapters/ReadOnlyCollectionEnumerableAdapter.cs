using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlyCollectionEnumerableAdapter<T> : ICollection<T>, IReadOnlyCollection<T>
    {
        public IEnumerable<T> Elements { get; }
        public int Count { get; }
        
        public ReadOnlyCollectionEnumerableAdapter(IEnumerable<T> elements, int count)
        {
            Elements = elements;
            Count = count;
        }

        public IEnumerator<T> GetEnumerator() => Elements.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();

        public void Add(T item) => throw new NotSupportedException();
        public void Clear() => throw new NotSupportedException();
        public bool Contains(T item) => Elements.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            Elements.CopyTo(array, arrayIndex);

        public bool Remove(T item) => throw new NotSupportedException();
        public bool IsReadOnly => true;
    }
}
