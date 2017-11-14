using Dotnet.Func.Ext.Collections.Adapters;
using Dotnet.Func.Ext.Collections.Lazy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections
{
    public static class LookupExtensions
    {
        public static IEnumerable<valueˈ> Get<keyˈ, valueˈ>(this ILookup<keyˈ, valueˈ> that, keyˈ key) => that[key];

        // todo desc
        public static IReadOnlySet<key> GetKeys<key, value>(this ILookup<key, value> that) =>
            new ReadOnlySetLookupAdapter<key, value>(that);

        public static IReadOnlyCollection<IEnumerable<valueˈ>> GetValues<keyˈ, valueˈ>(this ILookup<keyˈ, valueˈ> that) =>
            new ReadOnlyCollectionEnumerableAdapter<IEnumerable<valueˈ>>(that.Select(e => e), that.Count);

        public static ILookup<keyˈ, valueˈ> Filter<keyˈ, valueˈ>(this ILookup<keyˈ, valueˈ> that, Func<keyˈ, bool> f) =>
            new LookupFiltered<keyˈ, valueˈ>(that, f);

        public static ILookup<keyˈ, valueˈ> MapValues<keyˈ, innerValueˈ, valueˈ>(this ILookup<keyˈ, innerValueˈ> that, Func<keyˈ, innerValueˈ, valueˈ> f) =>
            new LookupProjected<keyˈ, innerValueˈ, valueˈ>(that, f);

        public static ILookup<keyˈ, valueˈ> MapValues<keyˈ, innerValueˈ, valueˈ>(this ILookup<keyˈ, innerValueˈ> that, Func<innerValueˈ, valueˈ> f) =>
            that.MapValues((_, v) => f(v));
    }
}
