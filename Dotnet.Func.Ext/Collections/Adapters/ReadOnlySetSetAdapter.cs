using System.Collections;
using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlySetSetAdapter<elementˈ> : IReadOnlySet<elementˈ>
    {
        public ISet<elementˈ> InnerSet { get; }

        public ReadOnlySetSetAdapter(ISet<elementˈ> innerSet)
        {
            InnerSet = innerSet;
        }

        public int Count => InnerSet.Count;
        public bool Contains(elementˈ item) => InnerSet.Contains(item);
        public IEnumerator<elementˈ> GetEnumerator() => InnerSet.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
