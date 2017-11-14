using System.Collections;
using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlyDictionaryDictionaryAdapter<K, V> : IReadOnlyDictionary<K, V>
    {
        public IDictionary<K, V> InnerDictionary { get; }

        public ReadOnlyDictionaryDictionaryAdapter(IDictionary<K, V> e) { InnerDictionary = e; }

        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => InnerDictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => InnerDictionary.Count;
        public V this[K i] => InnerDictionary[i];
        public bool ContainsKey(K key) => InnerDictionary.ContainsKey(key);
        public bool TryGetValue(K key, out V value) => InnerDictionary.TryGetValue(key, out value);
        public IEnumerable<K> Keys => InnerDictionary.Keys;
        public IEnumerable<V> Values => InnerDictionary.Values;
    }
}
