namespace Dotnet.Func.Ext.Collections
{
    using System;
    using System.Collections.Generic;

    public static class Initializers
    {
        /// <example>
        /// var numbers = new Initializer{int, string, NumberDescription}((index, name) => new NumberDescription(index, name))
        ///     { { 1, "one" }, { 2, "two" }, { 3, "three" } };
        /// </example>
        public class Initializer<inA, inB, outˈ>
        {
            private Func<inA, inB, outˈ> _map;

            private List<outˈ> _list;

            public Initializer(Func<inA, inB, outˈ> map)
            {
                _map = map;
                _list = new List<outˈ>();
            }

            public void Add(inA a1, inB a2)
            {
                _list.Add(_map(a1, a2));
            }

            public IReadOnlyList<outˈ> Value => _list;
        }

        /// <example>
        /// var numbers = new Initializer{int, string, bool, StrangeNumberDescription}((index, name, isOdd) => new StrangeNumberDescription(index, name, isOdd))
        ///     { { 1, "one", true }, { 2, "two", false }, { 3, "three", true } };
        /// </example>
        public class Initializer<inA, inB, inC, outˈ>
        {
            private Func<inA, inB, inC, outˈ> _map;

            private List<outˈ> _list;

            public Initializer(Func<inA, inB, inC, outˈ> map)
            {
                _map = map;
                _list = new List<outˈ>();
            }

            public void Add(inA a1, inB a2, inC a3)
            {
                _list.Add(_map(a1, a2, a3));
            }

            public IReadOnlyList<outˈ> Value => _list;
        }
    }
}
