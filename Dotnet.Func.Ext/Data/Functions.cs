using System;

namespace Dotnet.Func.Ext.Data
{
    using static Data.Units;
    using static Data.Ctors;
    using static Core.Functions;
    using Core;

    public static partial class Ctors
    {
        /// <summary>
        /// The only lazy injection
        /// </summary>
        public static Lazy<val> Lazy<val>(Func<Unit, val> v) => new Lazy<val>(v.IsoFunc());

        /// <summary>
        /// The Func_1 injection
        /// </summary>
        public static Func<a, b> Func<a, b>(Func<a, b> fˈ) => fˈ;
        /// <summary>
        /// The Func_2 injection
        /// </summary>
        public static Func<a, b, c> Func<a, b, c>(Func<a, b, c> fˈ) => fˈ;
        /// <summary>
        /// The Func_2 injection
        /// </summary>
        public static Func<a, b, c, d> Func<a, b, c, d>(Func<a, b, c, d> fˈ) => fˈ;
        /// <summary>
        /// The Func_4 injection
        /// </summary>
        public static Func<a, b, c, d, e> Func<a, b, c, d, e>(Func<a, b, c, d, e> fˈ) => fˈ;
        /// <summary>
        /// The Func_5 injection
        /// </summary>
        public static Func<a, b, c, d, e, f> Func<a, b, c, d, e, f>(Func<a, b, c, d, e, f> fˈ) => fˈ;
    }

    public static partial class Dtors
    {
        /// <summary>
        /// Lazy projection
        /// </summary>
        public static Func<Unit, val> Lazy<val>(this Lazy<val> v) => _ => v.Value;
    }

    /// <summary>
    /// Some useful generic high-order functions (mostly)
    /// </summary>
    public static class Functions
    {
        #region Func

        public static class Func
        {
            /// <summary>
            /// Pointed functor unary operation
            /// </summary>
            public static Func<inˈ, outˈ> Pure<inˈ, outˈ>(outˈ value) => _ => value;
        }

        public struct ConstFunc<outˈ>
        {
            outˈ _v;

            public ConstFunc(outˈ v) { _v = v; }

            public Func<inˈ, outˈ> With<inˈ>() => Func.Pure<inˈ, outˈ>(_v);
        }

        /// <summary>
        /// Point operation as extension
        /// </summary>
        public static ConstFunc<outˈ> PureFunc<outˈ>(this outˈ value) => new ConstFunc<outˈ>(value);

        /// <summary>
        /// Contravariant functor over a function
        /// </summary>
        public static Func<inˈ, outˈ> CoMap<inˈ, inˈˈ, outˈ>(this Func<inˈˈ, outˈ> f, Func<inˈ, inˈˈ> g) => Compose(f, g);
        /// <summary>
        /// Contravariant functor over a two args function
        /// </summary>
        public static Func<inˈ, inˈ, outˈ> CoMap<inˈ, inˈˈ, outˈ>(this Func<inˈˈ, inˈˈ, outˈ> f, Func<inˈ, inˈˈ> g) =>
            (x, y) => f(g(x), g(y));

        /// <summary>
        /// Covariant functor over a function
        /// </summary>
        public static Func<inˈ, outˈˈ> Map<inˈ, outˈ, outˈˈ>(this Func<inˈ, outˈ> f, Func<outˈ, outˈˈ> g) => Compose(g, f);
        /// <summary>
        /// Covariant functor over a two args function
        /// </summary>
        public static Func<inˈ, inˈ, outˈˈ> Map<inˈ, outˈ, outˈˈ>(this Func<inˈ, inˈ, outˈ> f, Func<outˈ, outˈˈ> g) =>
            (x, y) => g(f(x, y));

        /// <summary>
        /// Monadic `join` operation
        /// </summary>
        public static Func<inˈ, outˈ> Join<inˈ, outˈ>(this Func<inˈ, Func<inˈ, outˈ>> that) => v => that(v)(v);

        /// <summary>
        /// Monadic `bind` operation
        /// </summary>
        public static Func<inˈ, outˈˈ> Bind<inˈ, outˈ, outˈˈ>(this Func<inˈ, outˈ> that, Func<outˈ, Func<inˈ, outˈˈ>> f) => that.Map(f).Join();

        /// <summary>
        /// Execute the function in function context (kinda `reader monad` but an app. functor)
        /// </summary>
        public static Func<inˈ, outˈˈ> Ap<inˈ, outˈ, outˈˈ>(this Func<inˈ, Func<outˈ, outˈˈ>> f, Func<inˈ, outˈ> a) => v => f(v)(a(v));
        
        public static Func<inˈ, outˈˈ> Select<inˈ, outˈ, outˈˈ>(this Func<inˈ, outˈ> that, Func<outˈ, outˈˈ> f) => that.Map(f);
        public static Func<inˈ, outˈˈˈ> SelectMany<inˈ, outˈ, outˈˈ, outˈˈˈ>(this Func<inˈ, outˈ> that, Func<outˈ, Func<inˈ, outˈˈ>> f, Func<outˈ, outˈˈ, outˈˈˈ> s) =>
            that.Bind(v => f(v).Map(vv => s(v, vv)));

        #endregion

        #region Lazy

        public static class Lazy
        {
        /// <summary>
        /// Pointed functor unary operation
        /// </summary>
            public static Lazy<val> Pure<val>(val value) => Lazy(_ => value);
        }

        /// <summary>
        /// Point operation as extension
        /// </summary>
        /// <typeparam name="val"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Lazy<val> PureLazy<val>(this val value) => Lazy.Pure(value);

        /// <summary>
        /// Map over a value returned by lazy
        /// </summary>
        public static Lazy<b> Map<a, b>(this Lazy<a> that, Func<a, b> f) => Lazy(_ => f(that.Value));

        /// <summary>
        /// Collapse nested lazy
        /// </summary>
        public static Lazy<a> Join<a>(this Lazy<Lazy<a>> that) => Lazy(_ => that.Value.Value);

        /// <summary>
        /// Monadic `bind` operation
        /// </summary>
        public static Lazy<b> Bind<a, b>(this Lazy<a> that, Func<a, Lazy<b>> f) => that.Map(f).Join();

        public static Lazy<outˈ> Select<val, outˈ>(this Lazy<val> that, Func<val, outˈ> f) => that.Map(f);
        public static Lazy<outˈ> SelectMany<val, valˈ, outˈ>(this Lazy<val> that, Func<val, Lazy<valˈ>> f, Func<val, valˈ, outˈ> s) =>
            that.Bind(v => f(v).Map(vv => s(v, vv)));


        #endregion
    }
}
