using Dotnet.Func.Ext.Collections.Casts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class DictionaryReadOnlyDictionaryAdapter<K, V> : IDictionary<K, V>
    {
        public IReadOnlyDictionary<K, V> InnerDictionary { get; }

        public DictionaryReadOnlyDictionaryAdapter(IReadOnlyDictionary<K, V> e) { InnerDictionary = e; }

        public V this[K key] { get => InnerDictionary[key]; set => throw new InvalidOperationException(); }
        public ICollection<K> Keys => InnerDictionary.GetKeys().ViewAsCollection();
        public ICollection<V> Values => InnerDictionary.GetValues().ViewAsCollection();
        public bool Contains(KeyValuePair<K, V> item) => InnerDictionary.Contains(item);
        public bool ContainsKey(K key) => InnerDictionary.ContainsKey(key);
        public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex) => InnerDictionary.CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => InnerDictionary.GetEnumerator();
        public bool TryGetValue(K key, out V value) => InnerDictionary.TryGetValue(key, out value);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public int Count => InnerDictionary.Count;

        public bool IsReadOnly => true;

        public void Add(K key, V value) => throw new InvalidOperationException();
        public void Add(KeyValuePair<K, V> item) => throw new InvalidOperationException();
        public void Clear() => throw new InvalidOperationException();
        public bool Remove(K key) => throw new InvalidOperationException();
        public bool Remove(KeyValuePair<K, V> item) => throw new InvalidOperationException();
    }
}
