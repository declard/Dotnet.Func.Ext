
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
            that.Select(x => KeyValuePair(f(x.Key), x.Value));

        /// <summary>
        /// Apply a transformation to the values of a dictionary-like enumerable
        /// </summary>
        public static IEnumerable<KeyValuePair<key, valueOut>> MapValues<key, valueIn, valueOut>(this IEnumerable<KeyValuePair<key, valueIn>> that, Func<valueIn, valueOut> f) =>
            that.Select(x => KeyValuePair(x.Key, f(x.Value)));

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
        /// Group enumerable into a dictionary
        /// </summary>
        /// <param name="that">Dictionary-like input</param>
        /// <param name="keySelector">Provides a key to group by</param>
        /// <param name="valueSelector">Takes a key of a group and all elements of the group and produces an output value</param>
        /// <returns></returns>
        public static Dictionary<keyˈ, valueˈ> GroupToDictionary<inˈ, keyˈ, valueˈ>(
            this IEnumerable<inˈ> that,
            Func<inˈ, keyˈ> keySelector,
            Func<keyˈ, IEnumerable<inˈ>, valueˈ> valueSelector) =>
            that.GroupBy(keySelector).ToDictionary(g => g.Key, g => valueSelector(g.Key, g));

        /// <summary>
        /// Extract contained values for all Opts in the dict-like values, skipping pairs with empty Opts
        /// </summary>
        /// <example>collectValuesSome [(1, Some 4), (2, None), (3, Some 2)] → [(1, 4), (3, 2)]</example>
        public static IEnumerable<KeyValuePair<key, value>> CollectValuesSome<key, value>(this IEnumerable<KeyValuePair<key, Opt<value>>> that) =>
            that.Where(x => x.Value.IsSome()).MapValues(Optionals.Get);

        /// <summary>
        /// Standard ToDictionary analogue with merging function
        /// </summary>
        /// <example>toDictionary [(1, 1), (2, 2), (3, 3)] (λv.mod v 2) toString (λk l r.l ++ r) → [(1, "13") (0, "2")]</example>
        public static Dictionary<keyˈ, valueˈ> ToDictionary<inˈ, keyˈ, valueˈ>(
            this IEnumerable<inˈ> that,
            Func<inˈ, keyˈ> keySelector,
            Func<inˈ, valueˈ> valueSelector,
            Func<keyˈ, valueˈ, valueˈ, valueˈ> valueMerge)
        {
            var count = (that as IReadOnlyCollection<inˈ>)?.Count ?? 0;

            var result = new Dictionary<keyˈ, valueˈ>(count);

            foreach (var e in that)
            {
                var key = keySelector(e);
                var newValue = valueSelector(e);

                var oldValue = result.TryGetValue(key);

                if (oldValue.IsSome())
                    result[key] = valueMerge(key, oldValue.Some(), newValue);
                else
                    result.Add(key, newValue);
            }

            return result;
        }
    }
}
