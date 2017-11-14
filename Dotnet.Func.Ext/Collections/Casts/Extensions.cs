namespace Dotnet.Func.Ext.Collections.Casts
{
    using System.Collections.Generic;
    using Dotnet.Func.Ext.Collections.Adapters;

    public static class Extensions
    {
        public static IReadOnlyCollection<val> ViewAsReadOnlyCollection<val>(this ICollection<val> that) =>
            new ReadOnlyCollectionCollectionAdapter<val>(that);

        public static IReadOnlyList<val> ViewAsReadOnlyList<val>(this IList<val> that) =>
            new ReadOnlyListListAdapter<val>(that);

        public static IReadOnlyDictionary<key, value> ViewAsReadOnlyDictionary<key, value>(this IDictionary<key, value> that) =>
            new ReadOnlyDictionaryDictionaryAdapter<key, value>(that);

        public static IReadOnlySet<element> ViewAsReadOnlySet<element>(this ISet<element> that) =>
            new ReadOnlySetSetAdapter<element>(that);

        
        public static ICollection<val> ViewAsCollection<val>(this IReadOnlyCollection<val> that) =>
            new CollectionReadOnlyCollectionAdapter<val>(that);

        public static IList<val> ViewAsList<val>(this IReadOnlyList<val> that) =>
            new ListReadOnlyListAdapter<val>(that);

        public static IDictionary<key, value> ViewAsDictionary<key, value>(this IReadOnlyDictionary<key, value> that) =>
            new DictionaryReadOnlyDictionaryAdapter<key, value>(that);

        public static ISet<element> ViewAsSet<element>(this IReadOnlySet<element> that) =>
            new SetReadOnlySetAdapter<element>(that);

        public static IReadOnlyCollection<val> AsReadOnlyCollection<val>(this IReadOnlyCollection<val> that) => that;

        public static IReadOnlyList<val> AsReadOnlyList<val>(this IReadOnlyList<val> that) => that;

        public static IReadOnlyDictionary<key, value> AsReadOnlyDictionary<key, value>(this IReadOnlyDictionary<key, value> that) => that;

        public static IReadOnlySet<element> AsReadOnlySet<element>(this IReadOnlySet<element> that) => that;
    }
}
