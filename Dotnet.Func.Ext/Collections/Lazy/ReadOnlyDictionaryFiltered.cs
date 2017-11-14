using Dotnet.Func.Ext.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Lazy
{
    public class ReadOnlyDictionaryFiltered<keyˈ, valueˈ> : ReadOnlyDictionaryBase<keyˈ, valueˈ>
    {
        public IReadOnlyDictionary<keyˈ, valueˈ> InnerDictionary { get; }
        public Func<keyˈ, valueˈ, bool> Filter { get; }
        private int? _count;

        public ReadOnlyDictionaryFiltered(
            IReadOnlyDictionary<keyˈ, valueˈ> innerDictionary,
            Func<keyˈ, valueˈ, bool> filter)
        {
            InnerDictionary = innerDictionary;
            Filter = filter;
            _count = null;
        }

        protected override int Count => _count ?? (_count = InnerDictionary.Count(kvp => kvp.Case(Filter))).Value;

        protected override IEnumerator<KeyValuePair<keyˈ, valueˈ>> GetEnumerator() =>
            InnerDictionary.Where(kvp => kvp.Case(Filter)).GetEnumerator();

        protected override bool TryGetValue(keyˈ key, out valueˈ value)
        {
            if (!InnerDictionary.TryGetValue(key, out value))
                return false;

            if (Filter(key, value))
                return true;

            value = default(valueˈ);
            return false;
        }
    }
}
