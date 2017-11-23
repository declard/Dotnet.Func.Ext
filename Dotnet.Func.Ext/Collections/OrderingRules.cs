namespace Dotnet.Func.Ext.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class OrderingRules<selectorKey, element> : IEnumerable
    {
        private abstract class Orderer
        {
            public abstract IOrderedEnumerable<element> Run(IOrderedEnumerable<element> that, bool isDesc);
            public abstract IOrderedQueryable<element> Run(IOrderedQueryable<element> that, bool isDesc);
        }

        private class Orderer<F> : Orderer
        {
            public Expression<Func<element, F>> Selector { get; set; }

            public override IOrderedEnumerable<element> Run(IOrderedEnumerable<element> that, bool isDesc) =>
                isDesc ? that.ThenByDescending(Selector.Compile()) : that.ThenBy(Selector.Compile());

            public override IOrderedQueryable<element> Run(IOrderedQueryable<element> that, bool isDesc) =>
                isDesc ? that.ThenByDescending(Selector) : that.ThenBy(Selector);
        }

        private readonly Dictionary<selectorKey, Orderer> _orderers = new Dictionary<selectorKey, Orderer>();

        public void Add<selectedValue>(selectorKey key, Expression<Func<element, selectedValue>> selector)
        {
            _orderers.Add(key, new Orderer<selectedValue> { Selector = selector });
        }

        public IOrderedEnumerable<element> Run(IOrderedEnumerable<element> that, selectorKey ley, bool isDesc) =>
            _orderers[ley].Run(that, isDesc);

        public IOrderedQueryable<element> Run(IOrderedQueryable<element> that, selectorKey key, bool isDesc) =>
            _orderers[key].Run(that, isDesc);
        
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
