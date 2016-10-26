
namespace Dotnet.Func.Ext.Collections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Data.Ctors;
    using static Data.Optionals;
    using static Data.Units;
    using static Exceptions;
    using Data;

    public static class DictionaryExtensions
    {
        private class TransformedDictionary<key, valueIn, valueOut> : IReadOnlyDictionary<key, valueOut>
        {
            private readonly IReadOnlyDictionary<key, valueIn> _dict;
            private readonly Func<key, valueIn, valueOut> _f;

            public TransformedDictionary(IReadOnlyDictionary<key, valueIn> dict, Func<key, valueIn, valueOut> f)
            {
                _dict = dict;
                _f = f;
            }

            public IEnumerator<KeyValuePair<key, valueOut>> GetEnumerator() => AsEnumerable.GetEnumerator();

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            public int Count => _dict.Count;

            public bool ContainsKey(key key) => _dict.ContainsKey(key);

            public bool TryGetValue(key key, out valueOut value)
            {
                valueIn stored;
                if (_dict.TryGetValue(key, out stored))
                {
                    value = _f(key, stored);
                    return true;
                }

                value = default(valueOut);
                return false;
            }

            public valueOut this[key key] => _f(key, _dict[key]);

            public IEnumerable<key> Keys => _dict.Keys;
            public IEnumerable<valueOut> Values => AsEnumerable.GetValues();

            private IEnumerable<KeyValuePair<key, valueOut>> AsEnumerable => _dict.Select(kvp => KeyValuePair(kvp.Key, _f(kvp.Key, kvp.Value)));
        }

        /// <summary>
        /// Optimized ToDictionary: preallocates space
        /// </summary>
        public static Dictionary<key, value> ToDictionary<key, value>(this IReadOnlyCollection<KeyValuePair<key, value>> that)
        {
            if (that == null)
                return null;

            var result = new Dictionary<key, value>(that.Count);

            foreach (var e in that)
                try
                {
                    result.Add(e.Key, e.Value);
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException($"Dictionary.Add({e.Key}, {e.Value}) fail: the key has already been associated with the value {result.Get(e.Key)}", ex);
                }

            return result;
        }

        /// <summary>
        /// To be used instead of an indexer
        /// </summary>
        /// <example><code>collection.Select(dictionary.Get)</code> instead of <code>collection.Select(v => dictionary[v])</code></example>
        public static value Get<key, value>(this IReadOnlyDictionary<key, value> that, key k) => that[k];

        /// <summary>
        /// To be used instead of an indexer (pair version)
        /// </summary>
        /// <example><code>collection.Select(dictionary.Set)</code> instead of <code>collection.Select(kvp => dictionary[kvp.Key] = kvp.Value)</code></example>
        public static void Set<key, value>(this IDictionary<key, value> that, KeyValuePair<key, value> pair) => that.Set(pair.Key, pair.Value);
        /// <summary>
        /// To be used instead of an indexer
        /// </summary>
        public static void Set<keyˈ, valueˈ>(this IDictionary<keyˈ, valueˈ> that, keyˈ key, valueˈ value) => that[key] = value;

        /// <summary>
        /// Try to get value by key or use value source if failed
        /// </summary>
        public static value GetValueOr<key, value>(this IReadOnlyDictionary<key, value> dst, key k, Func<Unit, value> defaultSource) =>
           dst.TryGetValue(k).GetValueOr(defaultSource);

        /// <summary>
        /// Try to get value by key or construct a default value if failed
        /// </summary>
        public static value GetValueOrDefault<key, value>(this IReadOnlyDictionary<key, value> dst, key k) =>
            dst.GetValueOr(k, _ => default(value));

        /// <summary>
        /// Try get value by key in a safe manner
        /// </summary>
        public static Opt<value> TryGetValue<key, value>(this IReadOnlyDictionary<key, value> dst, key k)
        {
            if (dst == null || k == null)
                return None<value>();

            value v;

            return dst.TryGetValue(k, out v) ? Some(v) : None<value>();
        }

        /// <summary>
        /// Dictionary version of MapKeys with merging function for collisions resolution
        /// </summary>
        public static IReadOnlyDictionary<keyOut, value> MapKeys<keyIn, value, keyOut>(this IReadOnlyDictionary<keyIn, value> that, Func<keyIn, keyOut> f, Func<keyOut, value, value, value> m) =>
            that.MapKeys(f).Select(Tuples.ToPair).ToDictionary(Dtors.Left, Dtors.Right, m);

        /// <summary>
        /// Dictionary version of MapValues (with key-dependent transformation)
        /// </summary>
        public static IReadOnlyDictionary<key, valueOut> MapValues<key, valueIn, valueOut>(this IReadOnlyDictionary<key, valueIn> that, Func<key, valueIn, valueOut> f) =>
            new TransformedDictionary<key, valueIn, valueOut>(that, f);

        /// <summary>
        /// Dictionary version of MapValues
        /// </summary>
        public static IReadOnlyDictionary<key, valueOut> MapValues<key, valueIn, valueOut>(this IReadOnlyDictionary<key, valueIn> that, Func<valueIn, valueOut> f) =>
            that.MapValues((_, v) => f(v));

        /// <summary>
        /// Analogue for Union, but by keys
        /// </summary>
        public static Dictionary<keyˈ, valueˈ> RightUnionByKeys<keyˈ, valueˈ>(this IEnumerable<IReadOnlyDictionary<keyˈ, valueˈ>> that) =>
           that.Merge((key, oldValue, newValue) => newValue);

        /// <summary>
        /// Merge a list of dictionaries with merging function for collisions resolution
        /// </summary>
        public static Dictionary<key, value> Merge<key, value>(this IEnumerable<IReadOnlyDictionary<key, value>> that, Func<key, value, value, value> merge)
        {
            var concatDictionary = new Dictionary<key, value>();

            foreach (var kvp in that.SelectMany(dictionary => dictionary))
            {
                var existing = concatDictionary.TryGetValue(kvp.Key);

                if (existing.IsSome())
                    concatDictionary[kvp.Key] = merge(kvp.Key, existing.Some(), kvp.Value);
                else
                    concatDictionary[kvp.Key] = kvp.Value;
            }

            return concatDictionary;
        }

        /// <summary>
        /// Add a dictionary-like range into the dictionary
        /// </summary>
        public static Unit AddRange<key, value>(this Dictionary<key, value> that, IEnumerable<KeyValuePair<key, value>> another)
        {
            another.Foreach(kvp => that.AddOrFail(kvp.Key, kvp.Value));

            return Unit();
        }

        /// <summary>
        /// Add a value into the dictionary or fail with informative message :]
        /// </summary>
        public static Unit AddOrFail<key, value>(this Dictionary<key, value> that, key k, value v)
        {
            that.AddWith(k, v, (kˈ, vˈ, nv) => Throw<value>(new ArgumentException($"Dictionary.Add({kˈ}, {nv}) failed: the key has already been associated with the value {vˈ}")));

            return Unit();
        }

        /// <summary>
        /// Add a value into the dictionary or merge values on collision
        /// </summary>
        public static Opt<value> AddWith<key, value>(this Dictionary<key, value> that, key k, value v, Func<key, value, value, value> merge)
        {
            var existing = that.TryGetValue(k);

            if (existing.IsNone())
                that.Add(k, v);
            else
                that[k] = merge(k, existing.Some(), v);

            return existing;
        }

        /// <summary>
        /// Universal dictionary mutation operation
        /// Transformations:
        /// `None → None` - no action
        /// `None → Some v` - add v as k-th element
        /// `Some _ → None` - delete k-th element
        /// `Some _ → Some v` - replace k-th elementh with v
        /// </summary>
        public static Unit Alter<key, value>(this Dictionary<key, value> that, Func<Opt<value>, Opt<value>> f, key k)
        {
            var existing = that.TryGetValue(k);

            var newValue = f(existing);

            if (newValue.IsSome())
                that[k] = newValue.Some();
            else if (existing.IsSome())
                that.Remove(k);

            return Unit();
        }
    }
}
