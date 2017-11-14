using System.Collections;
using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlyCollectionCollectionAdapter<T> : IReadOnlyCollection<T>
    {
        public ICollection<T> InnerCollection { get; }

        public ReadOnlyCollectionCollectionAdapter(ICollection<T> e) { InnerCollection = e; }

        public IEnumerator<T> GetEnumerator() => InnerCollection.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => InnerCollection.Count;
    }
}
