using System.Collections;
using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlyListListAdapter<T> : IReadOnlyList<T>
    {
        public IList<T> InnerList { get; }

        public ReadOnlyListListAdapter(IList<T> e) { InnerList = e; }

        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => InnerList.Count;
        public T this[int i] => InnerList[i];
    }
}
