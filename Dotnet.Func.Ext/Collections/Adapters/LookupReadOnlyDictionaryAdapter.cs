using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class LookupReadOnlyDictionaryAdapter<keyˈ, valueˈ, dictionaryˈ, collectionˈ> :
        LookupReadOnlyDictionaryAdapterBase<keyˈ, valueˈ, dictionaryˈ, collectionˈ>
        where dictionaryˈ : IReadOnlyDictionary<keyˈ, collectionˈ>
        where collectionˈ : IEnumerable<valueˈ>
    {
        public LookupReadOnlyDictionaryAdapter(dictionaryˈ dictionary) : base(dictionary) { }

        protected override IEnumerable<valueˈ> Project(collectionˈ collection) => collection;
    }

    public class LookupReadOnlyDictionaryAdapter<keyˈ, valueˈ, collectionˈ> :
        LookupReadOnlyDictionaryAdapterBase<keyˈ, valueˈ, IReadOnlyDictionary<keyˈ, collectionˈ>, collectionˈ>
    {
        public Func<collectionˈ, IEnumerable<valueˈ>> Projection { get; }

        public LookupReadOnlyDictionaryAdapter(
            IReadOnlyDictionary<keyˈ, collectionˈ> dictionary,
            Func<collectionˈ, IEnumerable<valueˈ>> projection) : base(dictionary)
        {
            Projection = projection;
        }

        protected override IEnumerable<valueˈ> Project(collectionˈ collection) => Projection(collection);
    }

    public abstract class LookupReadOnlyDictionaryAdapterBase<keyˈ, valueˈ, dictionaryˈ, collectionˈ> : ILookup<keyˈ, valueˈ>
        where dictionaryˈ : IReadOnlyDictionary<keyˈ, collectionˈ>
    {
        public dictionaryˈ InnerDictionary { get; }

        public LookupReadOnlyDictionaryAdapterBase(dictionaryˈ dictionary)
        {
            InnerDictionary = dictionary;
        }
        
        public IEnumerable<valueˈ> this[keyˈ key] =>
            InnerDictionary.TryGetValue(key, out var value) ? Project(value) : Enumerable.Empty<valueˈ>();

        public int Count => InnerDictionary.Count;

        public bool Contains(keyˈ key) => InnerDictionary.ContainsKey(key);

        public IEnumerator<IGrouping<keyˈ, valueˈ>> GetEnumerator() =>
            InnerDictionary.AsEnumerable().MapValues(Project).Select(Grouping.Create).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected abstract IEnumerable<valueˈ> Project(collectionˈ collection);
    }
}
