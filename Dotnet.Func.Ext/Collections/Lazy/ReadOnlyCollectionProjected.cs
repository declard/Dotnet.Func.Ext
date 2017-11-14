using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Lazy
{
    public class ReadOnlyCollectionProjected<innerValueˈ, valueˈ> : IReadOnlyCollection<valueˈ>
    {
        public IReadOnlyCollection<innerValueˈ> InnerCollection { get; }
        public Func<innerValueˈ, valueˈ> Projection { get; }

        public ReadOnlyCollectionProjected(
            IReadOnlyCollection<innerValueˈ> innerCollection,
            Func<innerValueˈ, valueˈ> projection)
        {
            InnerCollection = innerCollection;
            Projection = projection;
        }

        public int Count => InnerCollection.Count;

        public IEnumerator<valueˈ> GetEnumerator() =>
            InnerCollection.Select(Projection).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
