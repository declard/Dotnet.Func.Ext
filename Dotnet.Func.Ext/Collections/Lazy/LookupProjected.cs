using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Lazy
{
    public class LookupProjected<keyˈ, innerValueˈ, valueˈ> : ILookup<keyˈ, valueˈ>
    {
        public ILookup<keyˈ, innerValueˈ> InnerLookup { get; }
        public Func<keyˈ, innerValueˈ, valueˈ> Projection { get; }

        public LookupProjected(
            ILookup<keyˈ, innerValueˈ> innerLookup,
            Func<keyˈ, innerValueˈ, valueˈ> projection)
        {
            InnerLookup = innerLookup;
            Projection = projection;
        }

        public IEnumerable<valueˈ> this[keyˈ key] => InnerLookup[key].Map(key, Projection);

        public int Count => InnerLookup.Count;

        public bool Contains(keyˈ key) => InnerLookup.Contains(key);

        public IEnumerator<IGrouping<keyˈ, valueˈ>> GetEnumerator() =>
            InnerLookup.Select(g => g.Map(g.Key, (k, v) => Projection(k, v))).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
