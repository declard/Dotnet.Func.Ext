using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlySetLookupAdapter<keyˈ, valueˈ> : IReadOnlySet<keyˈ>
    {
        public ILookup<keyˈ, valueˈ> InnerLookup { get; }

        public ReadOnlySetLookupAdapter(ILookup<keyˈ, valueˈ> innerLookup)
        {
            InnerLookup = innerLookup;
        }

        public int Count => InnerLookup.Count;
        public bool Contains(keyˈ item) => InnerLookup.Contains(item);
        public IEnumerator<keyˈ> GetEnumerator() =>
            InnerLookup.GetKeys().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
