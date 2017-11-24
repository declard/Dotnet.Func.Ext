namespace Dotnet.Func.Ext.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class QueryOrderingRules<selectorKey, element> : IEnumerable
    {
        private abstract class Orderer
        {
            public abstract IOrderedQueryable<element> Run(IOrderedQueryable<element> that, bool isDesc);
            public abstract IOrderedQueryable<element> Run(IQueryable<element> that, bool isDesc);
        }

        private class Orderer<key> : Orderer
        {
            private readonly Expression<Func<element, key>> _selector;

            public Orderer(Expression<Func<element, key>> selector) { _selector = selector; }

            public override IOrderedQueryable<element> Run(IOrderedQueryable<element> that, bool isDesc) =>
                isDesc ? that.ThenByDescending(_selector) : that.ThenBy(_selector);

            public override IOrderedQueryable<element> Run(IQueryable<element> that, bool isDesc) =>
                isDesc ? that.OrderByDescending(_selector) : that.OrderBy(_selector);
        }

        private readonly Dictionary<selectorKey, Orderer> _orderers = new Dictionary<selectorKey, Orderer>();

        public void Add<selectedValue>(selectorKey key, Expression<Func<element, selectedValue>> selector) =>
            _orderers.Add(key, new Orderer<selectedValue>(selector));

        public IOrderedQueryable<element> Run(IOrderedQueryable<element> that, selectorKey key, bool isDesc) =>
            _orderers[key].Run(that, isDesc);

        public IOrderedQueryable<element> Run(IQueryable<element> that, selectorKey key, bool isDesc) =>
            _orderers[key].Run(that, isDesc);

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
