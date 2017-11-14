using System.Collections.Generic;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class ReadOnlyDictionaryDelegatingAdapter<keyˈ, valueˈ, dictionaryˈ> : ReadOnlyDictionaryBase<keyˈ, valueˈ>
        where dictionaryˈ : IReadOnlyDictionary<keyˈ, valueˈ>
    {
        public dictionaryˈ InnerDictionary { get; }

        public ReadOnlyDictionaryDelegatingAdapter(dictionaryˈ dictionary)
        {
            InnerDictionary = dictionary;
        }

        protected override int Count => InnerDictionary.Count;

        protected override IEnumerator<KeyValuePair<keyˈ, valueˈ>> GetEnumerator() =>
            InnerDictionary.GetEnumerator();

        protected override bool TryGetValue(keyˈ key, out valueˈ value) =>
            InnerDictionary.TryGetValue(key, out value);
    }
}
