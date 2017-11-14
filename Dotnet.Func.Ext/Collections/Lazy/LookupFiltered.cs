using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Lazy
{
    public class LookupFiltered<keyˈ, valueˈ> : ILookup<keyˈ, valueˈ>
    {
        public ILookup<keyˈ, valueˈ> InnerLookup { get; }
        public Func<keyˈ, bool> Filter { get; }
        private int? _count;

        public LookupFiltered(
            ILookup<keyˈ, valueˈ> innerLookup,
            Func<keyˈ, bool> filter)
        {
            InnerLookup = innerLookup;
            Filter = filter;
            _count = null;
        }

        public IEnumerable<valueˈ> this[keyˈ key] => Filter(key) ? InnerLookup.Get(key) : Enumerable.Empty<valueˈ>();

        public int Count => _count ?? (_count = InnerLookup.GetKeys().Count(Filter)).Value;

        public bool Contains(keyˈ key) => Filter(key) && InnerLookup.Contains(key);

        public IEnumerator<IGrouping<keyˈ, valueˈ>> GetEnumerator() => InnerLookup.Where(g => Filter(g.Key)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
