namespace Dotnet.Func.Ext.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class OrderingRules<selectorKey, element> : IEnumerable
    {
        private abstract class Orderer
        {
            public abstract IOrderedEnumerable<element> Run(IOrderedEnumerable<element> that, bool isDesc);
            public abstract IOrderedEnumerable<element> Run(IEnumerable<element> that, bool isDesc);
        }

        private class Orderer<key> : Orderer
        {
            private readonly Func<element, key> _selector;

            public Orderer(Func<element, key> selector) { _selector = selector; }

            public override IOrderedEnumerable<element> Run(IOrderedEnumerable<element> that, bool isDesc) =>
                isDesc ? that.ThenByDescending(_selector) : that.ThenBy(_selector);

            public override IOrderedEnumerable<element> Run(IEnumerable<element> that, bool isDesc) =>
                isDesc ? that.OrderByDescending(_selector) : that.OrderBy(_selector);
        }

        private readonly Dictionary<selectorKey, Orderer> _orderers = new Dictionary<selectorKey, Orderer>();

        public void Add<selectedValue>(selectorKey key, Func<element, selectedValue> selector) =>
            _orderers.Add(key, new Orderer<selectedValue>(selector));

        public IOrderedEnumerable<element> Run(IOrderedEnumerable<element> that, selectorKey key, bool isDesc) =>
            _orderers[key].Run(that, isDesc);

        public IOrderedEnumerable<element> Run(IEnumerable<element> that, selectorKey key, bool isDesc) =>
            _orderers[key].Run(that, isDesc);

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }
}
