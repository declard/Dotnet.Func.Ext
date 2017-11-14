using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Lazy
{
    public class ReadOnlyDictionaryProjected<keyˈ, innerValueˈ, valueˈ> : ReadOnlyDictionaryBase<keyˈ, valueˈ>
    {
        public IReadOnlyDictionary<keyˈ, innerValueˈ> InnerDictionary { get; }
        public Func<keyˈ, innerValueˈ, valueˈ> Projection { get; }

        public ReadOnlyDictionaryProjected(
            IReadOnlyDictionary<keyˈ, innerValueˈ> innerDictionary,
            Func<keyˈ, innerValueˈ, valueˈ> projection)
        {
            InnerDictionary = innerDictionary;
            Projection = projection;
        }

        protected override int Count => InnerDictionary.Count;

        protected override IEnumerator<KeyValuePair<keyˈ, valueˈ>> GetEnumerator() =>
            InnerDictionary.AsEnumerable().MapValues(Projection).GetEnumerator();

        protected override bool TryGetValue(keyˈ key, out valueˈ value)
        {
            value = default(valueˈ);

            if (!InnerDictionary.TryGetValue(key, out var innerValue))
                return false;

            value = Projection(key, innerValue);
            return true;
        }
    }
}
