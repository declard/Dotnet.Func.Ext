namespace Dotnet.Func.Ext.Collections
{
    using Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using static Data.Ctors;
    using static Data.Optionals;

    public static class DictionaryLikeExtensions
    {
        /// <summary>
        /// Apply a transformation to the keys of a dictionary-like enumerable
        /// </summary>
        public static IEnumerable<KeyValuePair<keyOut, value>> MapKeys<keyIn, value, keyOut>(this IEnumerable<KeyValuePair<keyIn, value>> that, Func<keyIn, keyOut> f) =>
            that.Map(f, (fˈ, x) => KeyValuePair(fˈ(x.Key), x.Value));

        /// <summary>
        /// Apply a transformation to the values of a dictionary-like enumerable
        /// </summary>
        public static IEnumerable<KeyValuePair<key, valueOut>> MapValues<key, valueIn, valueOut>(this IEnumerable<KeyValuePair<key, valueIn>> that, Func<valueIn, valueOut> f) =>
            that.Map(f, (fˈ, x) => KeyValuePair(x.Key, fˈ(x.Value)));

        public static IEnumerable<KeyValuePair<key, valueOut>> MapValues<key, valueIn, valueOut>(this IEnumerable<KeyValuePair<key, valueIn>> that, Func<key, valueIn, valueOut> f) =>
            that.Map(f, (fˈ, x) => KeyValuePair(x.Key, fˈ(x.Key, x.Value)));

        /// <summary>
        /// Get keys of a dictionary-like enumerable
        /// </summary>
        public static IEnumerable<key> GetKeys<key, value>(this IEnumerable<KeyValuePair<key, value>> that)
            => that.Select(x => x.Key);

        /// <summary>
        /// Get values of a dictionary-like enumerable
        /// </summary>
        public static IEnumerable<value> GetValues<key, value>(this IEnumerable<KeyValuePair<key, value>> that) =>
            that.Select(x => x.Value);

        /// <summary>
        /// Get values of a dictionary-like enumerable ordering by keys
        /// </summary>
        public static IEnumerable<value> GetValuesOrdered<key, value>(this IEnumerable<KeyValuePair<key, value>> that) =>
            that.OrderBy(e => e.Key).GetValues();

        /// <summary>
        /// Enumerate dictionary-like into a dictionay
        /// </summary>
        public static Dictionary<key, value> ToDictionary<key, value>(this IEnumerable<KeyValuePair<key, value>> that) =>
             that?.ToDictionary(x => x.Key, x => x.Value);
        
        /// <summary>
        /// Extract contained values for all Opts in the dict-like values, skipping pairs with empty Opts
        /// </summary>
        /// <example>collectValuesSome [(1, Some 4), (2, None), (3, Some 2)] → [(1, 4), (3, 2)]</example>
        public static IEnumerable<KeyValuePair<key, value>> CollectValuesSome<key, value>(this IEnumerable<KeyValuePair<key, Opt<value>>> that) =>
            that.Where(x => x.Value.IsSome()).MapValues(Optionals.Get);

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
    }
}
