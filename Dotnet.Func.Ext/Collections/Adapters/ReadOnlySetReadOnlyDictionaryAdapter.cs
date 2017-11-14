using System.Collections;
using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlySetReadOnlyDictionaryAdapter<keyˈ, valueˈ> : IReadOnlySet<keyˈ>
    {
        public IReadOnlyDictionary<keyˈ, valueˈ> InnerDictionary { get; }

        public ReadOnlySetReadOnlyDictionaryAdapter(IReadOnlyDictionary<keyˈ, valueˈ> innerDictionary)
        {
            InnerDictionary = innerDictionary;
        }

        public int Count => InnerDictionary.Count;
        public bool Contains(keyˈ item) => InnerDictionary.ContainsKey(item);
        public IEnumerator<keyˈ> GetEnumerator() => InnerDictionary.Keys.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
