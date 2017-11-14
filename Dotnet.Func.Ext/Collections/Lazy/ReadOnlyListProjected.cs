using Dotnet.Func.Ext.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Lazy
{
    public class ReadOnlyListProjected<innerValueˈ, valueˈ> : IReadOnlyList<valueˈ>
    {
        public IReadOnlyList<innerValueˈ> InnerList { get; }
        public Func<int, innerValueˈ, valueˈ> Projection { get; }

        public ReadOnlyListProjected(
            IReadOnlyList<innerValueˈ> innerList,
            Func<int, innerValueˈ, valueˈ> projection)
        {
            InnerList = innerList;
            Projection = projection;
        }

        public valueˈ this[int index] => Projection(index, InnerList[index]);

        public int Count => InnerList.Count;

        public IEnumerator<valueˈ> GetEnumerator() =>
            InnerList.Select((v, i) => Projection(i, v)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
