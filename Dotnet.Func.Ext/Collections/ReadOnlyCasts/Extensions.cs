namespace Dotnet.Func.Ext.Collections.ReadOnlyCasts
{
    using System.Collections.Generic;

    public static class ReadOnlyCasts
    {
        #region Read-only wrappers preventing casts

        private class ROIReadOnlyCollection<T> : IReadOnlyCollection<T> { IReadOnlyCollection<T> _e; public ROIReadOnlyCollection(IReadOnlyCollection<T> e) { _e = e; } public IEnumerator<T> GetEnumerator() => _e.GetEnumerator(); System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator(); public int Count => _e.Count; }

        private class ROIReadOnlyList<T> : IReadOnlyList<T> { IReadOnlyList<T> _e; public ROIReadOnlyList(IReadOnlyList<T> e) { _e = e; } public IEnumerator<T> GetEnumerator() => _e.GetEnumerator(); System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator(); public int Count => _e.Count; public T this[int i] => _e[i]; }

        private class ROIReadOnlyDictionary<K, V> : IReadOnlyDictionary<K, V> { IReadOnlyDictionary<K, V> _e; public ROIReadOnlyDictionary(IReadOnlyDictionary<K, V> e) { _e = e; } public IEnumerator<KeyValuePair<K, V>> GetEnumerator() => _e.GetEnumerator(); System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator(); public int Count => _e.Count; public V this[K i] => _e[i]; public bool ContainsKey(K key) => _e.ContainsKey(key); public bool TryGetValue(K key, out V value) => _e.TryGetValue(key, out value); public IEnumerable<K> Keys => _e.Keys; public IEnumerable<V> Values => _e.Values; }

        #endregion

        public static IReadOnlyCollection<val> AsReadOnlyCollection<val>(this IReadOnlyCollection<val> that) =>
            (that as ROIReadOnlyCollection<val>) ?? new ROIReadOnlyCollection<val>(that);

        public static IReadOnlyList<val> AsReadOnlyList<val>(this IReadOnlyList<val> that) =>
            (that as ROIReadOnlyList<val>) ?? new ROIReadOnlyList<val>(that);

        public static IReadOnlyDictionary<key, value> AsReadOnlyDictionary<key, value>(this IReadOnlyDictionary<key, value> that) =>
            (that as ROIReadOnlyDictionary<key, value>) ?? new ROIReadOnlyDictionary<key, value>(that);
    }
}
