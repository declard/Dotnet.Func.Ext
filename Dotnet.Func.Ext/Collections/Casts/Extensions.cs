namespace Dotnet.Func.Ext.Collections.Casts
{
    using System.Collections.Generic;
    
    public static class Extensions
    {
        private class ROICollection<T> : IReadOnlyCollection<T> { ICollection<T> _e; public ROICollection(ICollection<T> e) { _e = e; } public IEnumerator<T> GetEnumerator() => _e.GetEnumerator(); System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator(); public int Count => _e.Count; }

        private class ROIList<T> : IReadOnlyList<T> { IList<T> _e; public ROIList(IList<T> e) { _e = e; } public IEnumerator<T> GetEnumerator() => _e.GetEnumerator(); System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator(); public int Count => _e.Count; public T this[int i] => _e[i]; }

        private class ROIDictionary<K, V> : IReadOnlyDictionary<K, V> { IDictionary<K, V> _e; public ROIDictionary(IDictionary<K, V> e) { _e = e; } public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => _e.GetEnumerator(); System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator(); public int Count => _e.Count; public V this[K i] => _e[i]; public bool ContainsKey(K key) => _e.ContainsKey(key); public bool TryGetValue(K key, out V value) => _e.TryGetValue(key, out value); public IEnumerable<K> Keys => _e.Keys; public IEnumerable<V> Values => _e.Values; }

        public static IReadOnlyCollection<val> AsReadOnlyCollection<val>(this ICollection<val> that) =>
            (that as ROICollection<val>) ?? new ROICollection<val>(that);

        public static IReadOnlyList<val> AsReadOnlyList<val>(this IList<val> that) =>
            (that as ROIList<val>) ?? new ROIList<val>(that);

        public static IReadOnlyDictionary<key, value> AsReadOnlyDictionary<key, value>(this IDictionary<key, value> that) =>
            (that as ROIDictionary<key, value>) ?? new ROIDictionary<key, value>(that);
    }
}
