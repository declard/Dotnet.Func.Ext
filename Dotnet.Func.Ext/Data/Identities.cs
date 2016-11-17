namespace Dotnet.Func.Ext.Data
{
    using System;
    using System.Collections;
    using static Ctors;
    using static Functions;
    using static Identities;
    using SCG = System.Collections.Generic;

    public static partial class Ctors
    {
        /// <summary>
        /// The only identity injection
        /// </summary>
        public static Identity<val> Identity<val>(val v) => Identities.Identity<val>.CreateIdentity(v);
    }

    public static partial class Dtors
    {
        /// <summary>
        /// Identity projection
        /// </summary>
        public static val Identity<val>(this Identity<val> v) => v.Case(Id);
    }

    /// <summary>
    /// Identity wrappers
    /// </summary>
    public static class Identities
    {
        /// <summary>
        /// An identity monad
        /// Contains a value. That's all it does.
        /// </summary>
        /// <typeparam name="val">Contained type</typeparam>
        public struct Identity<val> : SCG.IEnumerable<val>
        {
            private val _value;

            public static Identity<val> CreateIdentity(val value) => new Identity<val> { _value = value };

            /// <summary>
            /// Basic pattern matcher (closure-evading)
            /// </summary>
            public outˈ Case<valCtx, outˈ>(valCtx valCtxˈ, Func<valCtx, val, outˈ> f) => f(valCtxˈ, _value);

            public SCG.IEnumerator<val> GetEnumerator() => Collections.Enumerable.YieldOne(_value).GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public override string ToString() => $"Identity({_value})";
        }

        public static class Identity
        {
            /// <summary>
            /// Pointed functor unary operation
            /// </summary>
            public static Identity<val> Pure<val>(val value) => Identity(value);
        }

        /// <summary>
        /// Point operation as an extension
        /// </summary>
        public static Identity<val> PureIdentity<val>(this val that) => Identity.Pure(that);

        /// <summary>
        /// Basic pattern matcher
        /// </summary>
        public static outˈ Case<val, outˈ>(this Identity<val> that, Func<val, outˈ> f) => f(that.Identity());

        /// <summary>
        /// Functor map
        /// </summary>
        public static Identity<outˈ> Map<val, outˈ>(this Identity<val> that, Func<val, outˈ> f) => Identity(that.Case(f));

        /// <summary>
        /// Monadic join
        /// </summary>
        public static Identity<valˈ> Join<valˈ>(this Identity<Identity<valˈ>> v) => v.Identity();

        /// <summary>
        /// Monadic bind
        /// </summary>
        public static Identity<outˈ> Bind<valˈ, outˈ>(this Identity<valˈ> v, Func<valˈ, Identity<outˈ>> f) => v.Map(f).Join();

        /// <summary>
        /// Applicative functor lift
        /// </summary>
        public static Identity<cˈ> Lift<aˈ, bˈ, cˈ>(this Func<aˈ, bˈ, cˈ> f, Identity<aˈ> a, Identity<bˈ> b) =>
            Identity(f(a.Identity(), b.Identity()));

        /// <summary>
        /// Linq map analogue
        /// </summary>
        public static Identity<outˈ> Select<val, outˈ>(Identity<val> that, Func<val, outˈ> f) => that.Map(f);
        /// <summary>
        /// Linq bind analogue
        /// </summary>
        public static Identity<outˈ> SelectMany<val, valˈ, outˈ>(Identity<val> that, Func<val, Identity<valˈ>> f, Func<val, valˈ, outˈ> g) =>
            g.Lift(that, that.Bind(f));
    }
}
