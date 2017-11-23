namespace Dotnet.Func.Ext.Collections
{
    using Algebraic;
    using Data;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using static Data.Ctors;
    using static Data.Eithers;
    using static Data.Optionals;
    using static Data.Tuples;
    using static Data.Units;
    using static Data.Functions;
    using static Core.Functions;

    public static partial class EnumerableExtensions
    {
        #region Basics

        /// <summary>
        /// Move to the next enumerator item and get it or fail
        /// </summary>
        public static val GetNext<val>(this IEnumerator<val> that)
        {
            if (that.MoveNext())
                return that.Current;

            throw new InvalidOperationException("Trying to take next element from exhausted enumerator");
        }

        /// <summary>
        /// Zip two enumerables of the same length together
        /// </summary>
        public static IEnumerable<outˈ> ZipExact<left, right, outˈ>(this IEnumerable<left> that, IEnumerable<right> another, Func<left, right, outˈ> f)
        {
            using (var le = that.GetEnumerator())
            using (var re = another.GetEnumerator())
                while (true)
                {
                    var ln = le.MoveNext();
                    var rn = re.MoveNext();
                    if (!ln && !rn) yield break;
                    if (ln && rn) yield return f(le.Current, re.Current);
                    else throw new InvalidOperationException("Enumerables have different lengths");
                }
        }

        /// <summary>
        /// Cut enumerable into chunks of a specified size
        /// </summary>
        public static IEnumerable<IReadOnlyList<val>> ChunkBy<val>(this IEnumerable<val> that, int chunkSize, bool ignoreUnderfilled = false)
        {
            using (var e = that.GetEnumerator())
            {
                while (true)
                {
                    var chunk = new List<val>(chunkSize);

                    var indexInChunk = 0;

                    while (indexInChunk++ < chunkSize)
                    {
                        if (!e.MoveNext())
                        {
                            if (!ignoreUnderfilled)
                                yield return chunk;

                            yield break;
                        }

                        chunk.Add(e.Current);
                    }

                    yield return chunk;
                }
            }
        }

        /// <summary>
        /// Loops enumerable by itself infinitely
        /// </summary>
        /// <example>cycle [1,2,3] → [1,2,3,1,2,3,1,2..]</example>
        public static IEnumerable<val> Cycle<val>(this IEnumerable<val> that)
        {
            while (true)
            {
                foreach (var e in that)
                    yield return e;
            }
        }

        /// <summary>
        /// Fold enumerable from left to right preserving intermediate results as a new enumerable
        /// The seed is treated as the first result
        /// </summary>
        public static IEnumerable<outˈ> Scanl<inˈ, outˈ>(this IEnumerable<inˈ> that, outˈ seed, Func<outˈ, inˈ, outˈ> next)
        {
            var current = seed;

            foreach(var e in that)
            {
                yield return current;
                current = next(current, e);
            }
        }

        /// <summary>
        /// Interleaves two enumerables
        /// </summary>
        /// <example>
        /// interleave [1,2,3] [4,5,6] → [1,4,2,5,3,6]
        /// interleave [1] [2,3] → [1,2,3]
        /// interleave [1,2,3] [4] → [1,4,2,3]
        /// </example>
        public static IEnumerable<val> Interleave<val>(this IEnumerable<val> that, IEnumerable<val> another)
        {
            var thatEn = that.GetEnumerator();
            var anotherEn = another.GetEnumerator();

            while (true)
            {
                if (thatEn.MoveNext())
                    yield return thatEn.Current;

                if (!anotherEn.MoveNext())
                    break;

                yield return anotherEn.Current;
            }

            while (thatEn.MoveNext())
                yield return thatEn.Current;
        }

        /// <summary>
        /// *By version of Distinct function
        /// </summary>
        public static IEnumerable<val> DistinctBy<val, key>(this IEnumerable<val> that, Func<val, key> keySelector) =>
            that.Distinct(EqualityComparer<key>.Default.Map(keySelector));

        /// <summary>
        /// Comparer-based version of Max
        /// </summary>
        public static val Max<val>(this IEnumerable<val> that, IComparer<val> comparer) =>
            that.MaxBy(comparer.Compare, nameof(Max));

        /// <summary>
        /// Comparer-based version of Min
        /// </summary>
        public static val Min<val>(this IEnumerable<val> that, IComparer<val> comparer) =>
            that.MaxBy((l, r) => -comparer.Compare(l, r), nameof(Min));

        private static val MaxBy<val>(this IEnumerable<val> that, Func<val, val, int> comparer, string op)
        {
            using (var e = that.GetEnumerator())
            {
                if (!e.MoveNext())
                    throw new InvalidOperationException($"{op} is not applicable to empty sequences");

                var max = e.Current;

                while (e.MoveNext())
                {
                    var current = e.Current;

                    if (comparer(current, max) > 0)
                        max = current;
                }

                return max;
            }
        }

        /// <summary>
        /// Prepends an element to enumerable
        /// </summary>
        /// <example>cons [2,3,4] 1 → [1,2,3,4]</example>
        public static IEnumerable<val> Cons<val>(this IEnumerable<val> that, val element)
        {
            yield return element;
            foreach (var e in that)
                yield return e;
        }

        /// <summary>
        /// Prepends an element to enumerable
        /// </summary>
        /// <example>cons 1 [2,3,4] → [1,2,3,4] </example>
        public static IEnumerable<val> Cons<val>(this val that, IEnumerable<val> element) => element.Cons(that);

        /// <summary>
        /// Appends an element to enumerable
        /// </summary>
        /// <example>snoc [1,2,3] 4 → [1,2,3,4]</example>
        public static IEnumerable<val> Snoc<val>(this IEnumerable<val> that, val element)
        {
            foreach (var e in that)
                yield return e;
            yield return element;
        }

        /// <summary>
        /// Flattens enumerable of enumerables
        /// </summary>
        /// <example>flat [[1,2],[],[3],[4,5,6]] → [1,2,3,4,5,6]</example>
        public static IEnumerable<val> Flat<val>(this IEnumerable<IEnumerable<val>> that) => that.SelectMany(Id);

        /// <summary>
        /// Constructs a singletone enumerable
        /// </summary>
        /// <example>yieldOne 0 → [0]</example>
        public static IEnumerable<val> YieldOne<val>(this val value)
        {
            yield return value;
        }

        /// <summary>
        /// Replaces null by an empty enumerable
        /// </summary>
        /// <example>
        /// emptyIfNull null → []
        /// emptyIfNull [1,2] → [1,2]
        /// </example>
        public static IEnumerable<val> EmptyIfNull<val>(this IEnumerable<val> that) => that ?? System.Linq.Enumerable.Empty<val>();

        /// <summary>
        /// Replaces null by an empty IReadOnlyDictionary
        /// </summary>
        public static IReadOnlyDictionary<key, val> EmptyIfNull<key, val>(this IReadOnlyDictionary<key, val> that) => that ?? new Dictionary<key, val>();

        /// <summary>
        /// Replaces null by an empty IReadOnlyList
        /// </summary>
        public static IReadOnlyList<val> EmptyIfNull<val>(this IReadOnlyList<val> that) => that ?? new val[] { };

        /// <summary>
        /// Replaces null by an empty IReadOnlyCollection
        /// </summary>
        public static IReadOnlyCollection<val> EmptyIfNull<val>(this IReadOnlyCollection<val> that) => that ?? new val[] { };


        /// <summary>
        /// Takes everything except for the last element
        /// </summary>
        /// <example>init [1,2,3] → [1,2]</example>
        public static IEnumerable<val> Init<val>(this IEnumerable<val> that)
        {
            using (var e = that.GetEnumerator())
            {
                if (!e.MoveNext()) yield break;
                var current = e.Current;
                while (e.MoveNext())
                {
                    yield return current;
                    current = e.Current;
                }
            }
        }

        /// <summary>
        /// Inserts a separator between every neighbour elements
        /// </summary>
        /// <example>interpserse [1,2,3] 4 → [1,4,2,4,3]</example>
        public static IEnumerable<val> Intersperse<val>(this IEnumerable<val> that, val separator)
        {
            using (var e = that.GetEnumerator())
            {
                if (!e.MoveNext()) yield break;
                var current = e.Current;
                while (e.MoveNext())
                {
                    yield return separator;
                    yield return current;
                    current = e.Current;
                }
                yield return current;
            }
        }

        /// <summary>
        /// Builds an infinite enumerable with a builder function (`iterate` analogue with output transformation applied)
        /// </summary>
        /// <example>unfold 1 (λv.(v, v + 1)) → [1,2,3,4,5..]</example>
        public static IEnumerable<outˈ> Unfold<val, outˈ>(this val seed, Func<val, Pair<outˈ, val>> next)
        {
            var current = seed;
            while (true)
            {
                var n = next(current);
                yield return n.Left();
                current = n.Right();
            }
        }

        /// <summary>
        /// Builds an enumerable with a builder function (`generate` analogue with output transformation applied)
        /// </summary>
        /// <example>unfold 1 (λv.if v `lessThan` 5 then Some (v, v + 1) else None) → [1,2,3,4]</example>
        public static IEnumerable<outˈ> Unfold<val, outˈ>(this val seed, Func<val, Opt<Pair<outˈ, val>>> next)
        {
            var current = seed;
            while (true)
            {
                var n = next(current);

                if (!n.IsSome())
                    yield break;

                yield return n.Some().Left();
                current = n.Some().Right();
            }
        }

        /// <summary>
        /// Deconstructs enumerable as a list (warning! multiple enumeration immanent! though elements are valuated at most once)
        /// </summary>
        public static outˈ Case<val, outˈ>(this IEnumerable<val> that, Func<Unit, outˈ> Nil, Func<Pair<val, IEnumerable<val>>, outˈ> Cons)
        {
            using (var e = that.GetEnumerator())
            {
                if (e.MoveNext())
                    return Cons(Pair(e.Current, that.Skip(1)));

                return Nil(Unit());
            }
        }

        /// <summary>
        /// Duplicates each element into a KVP
        /// </summary>
        public static IEnumerable<KeyValuePair<inˈ, inˈ>> SelectPairs<inˈ>(this IEnumerable<inˈ> that) =>
            that.SelectPairs(Id, Id);

        /// <summary>
        /// Duplicates each element into a KVP and performs their mapping
        /// </summary>
        public static IEnumerable<KeyValuePair<key, value>> SelectPairs<inˈ, key, value>(
            this IEnumerable<inˈ> that,
            Func<inˈ, key> keySelector,
            Func<inˈ, value> valueSelector) => that.Select(element => KeyValuePair(keySelector(element), valueSelector(element)));

        /// <summary>
        /// Foreach function
        /// </summary>
        public static void Foreach<val>(this IEnumerable<val> that, Action<val> action) =>
            that.Foreach(action.AsFunc());

        /// <summary>
        /// Foreach function
        /// </summary>
        public static void Foreach<val>(this IEnumerable<val> that, Func<val, Unit> action)
        {
            foreach (var i in that)
                action(i);
        }

        /// <summary>
        /// Tries to get the first value in a safe manner
        /// </summary>
        public static Opt<val> TryGetFirst<val>(this IEnumerable<val> that)
        {
            using (var e = that.GetEnumerator())
                if (e.MoveNext())
                    return Some(e.Current);

            return None();
        }

        /// <summary>
        /// Tries to find the first matching value in a safe manner
        /// </summary>
        public static Opt<val> TryGetFirst<val>(this IEnumerable<val> that, Func<val, bool> pred) =>
            that.Where(pred).TryGetFirst();

        /// <summary>
        /// Tries to get single value in a safe manner
        /// </summary>
        public static Opt<val> TryGetSingle<val>(this IEnumerable<val> that)
        {
            using (var e = that.GetEnumerator())
            {
                if (!e.MoveNext())
                    return None();

                var result = e.Current;

                if (e.MoveNext())
                    return None();

                return result;
            }
        }

        /// <summary>
        /// Tries to find the only matching value in a safe manner
        /// </summary>
        public static Opt<val> TryGetSingle<val>(this IEnumerable<val> that, Func<val, bool> pred) =>
            that.Where(pred).TryGetSingle();

        // todo desc
        public static IEnumerable<outˈ> Map<valCtx, val, outˈ>(this IEnumerable<val> that, valCtx ctx, Func<valCtx, val, outˈ> f)
        {
            foreach (var element in that)
                yield return f(ctx, element);
        }

        #endregion

        #region Enumerations and generators

        /// <summary>
        /// Build an infinite enumeration from the value
        /// </summary>
        public static IEnumerable<val> Repeat<val>(this val that)
        {
            while (true)
                yield return that;
        }

        /// <summary>
        /// Builds an enumerable with a builder function (constantly applying transformation)
        /// </summary>
        public static IEnumerable<val> Generate<val>(this val seed, Func<val, Opt<val>> next) =>
            seed.Unfold(next.Map(f => f.Map(Pair)));

        /// <summary>
        /// Builds an infinite enumerable with a builder function (constantly applying transformation)
        /// </summary>
        /// <example>iterate 1 (λv.v * 2) → [1, 2, 4, 8, 16, 32..]</example>
        public static IEnumerable<val> Iterate<val>(this val seed, Func<val, val> next) =>
            seed.Unfold(next.Map(Pair));


        /// <summary>
        /// Unforgetful memoizer
        /// Thread unsafe
        /// </summary>
        public class Memoizer<outˈ> : IEnumerable<outˈ>
        {
            IEnumerator<outˈ> source;
            List<outˈ> memoized = new List<outˈ>();
            int enumerations = 0;

            private Memoizer() { }

            public static IEnumerable<outˈ> Fix(Func<IEnumerable<outˈ>, IEnumerable<outˈ>> f)
            {
                var m = new Memoizer<outˈ>();
                m.source = f(m).GetEnumerator();
                return m;
            }

            public IEnumerator<outˈ> GetEnumerator() => new Enumerator(this);
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            class Enumerator : IEnumerator<outˈ>
            {
                Memoizer<outˈ> memoizer;
                int position;

                public Enumerator(Memoizer<outˈ> memoizer)
                {
                    this.memoizer = memoizer;
                    this.position = -1;
                    memoizer.enumerations++;
                }

                public outˈ Current => memoizer.memoized[position];
                object IEnumerator.Current => Current;

                void IEnumerator.Reset() { position = -1; }

                bool IEnumerator.MoveNext()
                {
                    var m = memoizer.memoized;
                    var s = memoizer.source;
                    if (++position >= m.Count)
                    {
                        if (s.MoveNext())
                        {
                            m.Add(s.Current);
                        }
                        else return false;
                    }
                    return true;
                }

                void IDisposable.Dispose()
                {
                    var enumerations = --memoizer.enumerations;
                    if (enumerations == 0)
                        memoizer.source.Dispose();
                }
            }
        }


        /// <summary>
        /// Make enumerable transformation function recursive (circuit its output to its input)
        /// </summary>
        public static IEnumerable<outˈ> Fix<outˈ>(Func<IEnumerable<outˈ>, IEnumerable<outˈ>> f)
        {
            foreach (var v in f(Fix(f)))
            {
                yield return v;
            }
        }

        /// <summary>
        /// Make enumerable transformation function recursive (circuit its output to its input) through a memoizer
        /// </summary>
        public static IEnumerable<outˈ> FixMemoized<outˈ>(Func<IEnumerable<outˈ>, IEnumerable<outˈ>> f) =>
            Memoizer<outˈ>.Fix(f);

        #endregion

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

        #region ToCollection

        /// <summary>
        /// Enumerates into a list applying transformation for each element
        /// </summary>
        public static List<outˈ> ToList<inˈ, outˈ>(this IEnumerable<inˈ> that, Func<inˈ, outˈ> f) => that?.Select(f).ToList();

        /// <summary>
        /// Enumerates into a hash set
        /// </summary>
        public static HashSet<val> ToHashSet<val>(this IEnumerable<val> that) => that?.FeedTo(e => new HashSet<val>(e));
        
        #endregion
        
        /// <summary>
        /// Extract contained values for all Opts in the enumerable, skipping empty Opts
        /// </summary>
        /// <typeparam name="val">Contained type</typeparam>
        /// <param name="that">Enumerable object with Opts</param>
        /// <returns>Contained values</returns>
        public static IEnumerable<val> CollectSome<val>(this IEnumerable<Opt<val>> that) =>
            that.Where(x => x.IsSome()).Select(Optionals.Get);

        /// <summary>
        /// Extract contained lefts for all Eithers in the enumerable
        /// </summary>
        /// <param name="that">Enumerable object with Eithers</param>
        /// <returns>Contained lefts</returns>
        public static IEnumerable<left> CollectLeft<left, right>(this IEnumerable<Either<left, right>> that) =>
            that.Select(Eithers.TryGetLeft).CollectSome();

        /// <summary>
        /// Extract contained rights for all Eihers in the enumerable
        /// </summary>
        /// <param name="that">Enumerable object with Eithers</param>
        /// <returns>Contained rights</returns>
        public static IEnumerable<right> CollectRight<left, right>(this IEnumerable<Either<left, right>> that) =>
            that.Select(Eithers.TryGetRight).CollectSome();

        /// <summary>
        /// Project right values by vector projecting function
        /// </summary>
        /// <param name="input">Vector of left and right values</param>
        /// <param name="projector">Vector transformation over right values, preserving order and count</param>
        /// <returns>Vector with projected right values</returns>
        public static IReadOnlyList<Either<err, outˈ>> ProjectRights<inˈ, outˈ, err>(
               this IReadOnlyList<Either<err, inˈ>> input,
               Func<IReadOnlyList<inˈ>, IReadOnlyList<Either<err, outˈ>>> projector)
        {
            // Get only right values indexed through the whole sequence
            var indexedInnerInput = input
                .WithIndex()
                .MapValues(Eithers.TryGetRight)
                .CollectValuesSome()
                .ToDictionary();

            // Project right values preserving indexes
            var indexedInnerOutput = indexedInnerInput
                .Keys
                .Zip(projector(indexedInnerInput.Values.ToList()), KeyValuePair)
                .ToDictionary();

            // Replace rights from input by output
            return input.Select((inElement, i) => inElement.IsRight() ? indexedInnerOutput[i] : inElement.Map(_ => default(outˈ))).ToList();
        }

        public static void CopyTo<val>(this IEnumerable<val> that, val[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0 || arrayIndex > array.Length)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            
            var index = arrayIndex;
            foreach(var e in that)
            {
                if (index >= array.Length)
                    return;

                array[index] = e;

                index++;
            }
        }

        public static int IndexOf<element>(this IEnumerable<element> that, element item, IEqualityComparer<element> comparer = null)
        {
            if (that == null)
                throw new ArgumentNullException(nameof(that));

            comparer = comparer ?? EqualityComparer<element>.Default;

            var index = 0;
            foreach (var e in that)
            {
                if (comparer.Equals(e, item))
                    return index;

                index++;
            }

            return -1;
        }

        public static IEnumerable<KeyValuePair<int, element>> WithIndex<element>(this IEnumerable<element> that) =>
            that.Select(ValueKeyPair);

        public static IOrderedEnumerable<element> OrderBy<selectorKey, element>(this IEnumerable<element> that, OrderingRules<selectorKey, element> rules, selectorKey key, bool isDesc) =>
            rules.Run(that.OrderBy(_ => 0), key, isDesc);

        public static IOrderedQueryable<element> OrderBy<selectorKey, element>(this IQueryable<element> that, OrderingRules<selectorKey, element> rules, selectorKey key, bool isDesc) =>
            rules.Run(that.OrderBy(_ => 0), key, isDesc);

        public static IOrderedEnumerable<element> ThenBy<selectorKey, element>(this IOrderedEnumerable<element> that, OrderingRules<selectorKey, element> rules, selectorKey key, bool isDesc) =>
            rules.Run(that, key, isDesc);

        public static IOrderedQueryable<element> ThenBy<selectorKey, element>(this IOrderedQueryable<element> that, OrderingRules<selectorKey, element> rules, selectorKey key, bool isDesc) =>
            rules.Run(that, key, isDesc);
    }
}
