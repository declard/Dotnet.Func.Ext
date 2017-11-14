using System.Collections;
using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections
{
    public abstract class ReadOnlyDictionaryBase<keyˈ, valueˈ> : IReadOnlyDictionary<keyˈ, valueˈ>
    {
        protected abstract int Count { get; }
        protected abstract IEnumerator<KeyValuePair<keyˈ, valueˈ>> GetEnumerator();
        protected abstract bool TryGetValue(keyˈ key, out valueˈ value);

        IEnumerator<KeyValuePair<keyˈ, valueˈ>> IEnumerable<KeyValuePair<keyˈ, valueˈ>>.GetEnumerator() => GetEnumerator();
        int IReadOnlyCollection<KeyValuePair<keyˈ, valueˈ>>.Count => Count;
        bool IReadOnlyDictionary<keyˈ, valueˈ>.TryGetValue(keyˈ key, out valueˈ value) => TryGetValue(key, out value);
        valueˈ IReadOnlyDictionary<keyˈ, valueˈ>.this[keyˈ key] =>
            TryGetValue(key, out var value) ? value : throw new KeyNotFoundException();
        IEnumerable<keyˈ> IReadOnlyDictionary<keyˈ, valueˈ>.Keys => this.GetKeys();
        IEnumerable<valueˈ> IReadOnlyDictionary<keyˈ, valueˈ>.Values => this.GetValues();
        bool IReadOnlyDictionary<keyˈ, valueˈ>.ContainsKey(keyˈ key) => TryGetValue(key, out var _);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
