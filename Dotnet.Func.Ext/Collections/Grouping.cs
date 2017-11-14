using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections
{
    public class Grouping<keyˈ, valueˈ> : IGrouping<keyˈ, valueˈ>
    {
        private keyˈ _key;
        private IEnumerable<valueˈ> _values;

        public Grouping(keyˈ key, IEnumerable<valueˈ> values)
        {
            _key = key;
            _values = values;
        }

        public keyˈ Key => _key;
        public IEnumerator<valueˈ> GetEnumerator() => _values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class Grouping
    {
        public static Grouping<keyˈ, valueˈ> Create<keyˈ, valueˈ>(keyˈ key, IEnumerable<valueˈ> value) => new Grouping<keyˈ, valueˈ>(key, value);
        public static Grouping<keyˈ, valueˈ> Create<keyˈ, valueˈ>(KeyValuePair<keyˈ, IEnumerable<valueˈ>> kvp) => Create(kvp.Key, kvp.Value);

        public static Grouping<keyˈ, valueˈ> Map<ctxValue, keyˈ, innerValueˈ, valueˈ>(
            this IGrouping<keyˈ, innerValueˈ> that,
            ctxValue ctx,
            Func<ctxValue, innerValueˈ, valueˈ> f) => Create(that.Key, that.Map(ctx, f));

        public static Grouping<keyˈ, valueˈ> Map<keyˈ, innerValueˈ, valueˈ>(this IGrouping<keyˈ, innerValueˈ> that, Func<innerValueˈ, valueˈ> f) =>
            that.Map(f, (fˈ, e) => fˈ(e));
    }
}
