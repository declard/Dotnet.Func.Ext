using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Dotnet.Func.Ext.Collections.Adapters
{
    public class SetReadOnlySetAdapter<elementˈ> : ISet<elementˈ>
    {
        public IReadOnlySet<elementˈ> InnerSet { get; }

        public SetReadOnlySetAdapter(IReadOnlySet<elementˈ> innerSet)
        {
            InnerSet = innerSet;
        }

        public int Count => InnerSet.Count;
        public bool Contains(elementˈ item) => InnerSet.Contains(item);
        public IEnumerator<elementˈ> GetEnumerator() => InnerSet.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public void CopyTo(elementˈ[] array, int arrayIndex) => this.AsEnumerable().CopyTo(array, arrayIndex);
        public bool IsReadOnly => true;

        public bool IsProperSubsetOf(IEnumerable<elementˈ> other) => throw new NotImplementedException(); // todo implement
        public bool IsProperSupersetOf(IEnumerable<elementˈ> other) => throw new NotImplementedException();
        public bool IsSubsetOf(IEnumerable<elementˈ> other) => throw new NotImplementedException();
        public bool IsSupersetOf(IEnumerable<elementˈ> other) => throw new NotImplementedException();
        public bool Overlaps(IEnumerable<elementˈ> other) => throw new NotImplementedException();
        public bool SetEquals(IEnumerable<elementˈ> other) => throw new NotImplementedException();

        public bool Add(elementˈ item) => throw new InvalidOperationException();
        public bool Remove(elementˈ item) => throw new InvalidOperationException();
        public void Clear() => throw new InvalidOperationException();
        public void IntersectWith(IEnumerable<elementˈ> other) => throw new InvalidOperationException();
        public void ExceptWith(IEnumerable<elementˈ> other) => throw new InvalidOperationException();
        public void SymmetricExceptWith(IEnumerable<elementˈ> other) => throw new InvalidOperationException();
        public void UnionWith(IEnumerable<elementˈ> other) => throw new InvalidOperationException();
        void ICollection<elementˈ>.Add(elementˈ item) => throw new InvalidOperationException();
    }
}
