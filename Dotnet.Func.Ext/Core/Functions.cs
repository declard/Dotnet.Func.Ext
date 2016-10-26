using System;

namespace Dotnet.Func.Ext
{
    using static Data.Units;
    using static Data.Ctors;

    /// <summary>
    /// Some useful generic high-order functions (mostly)
    /// </summary>
    public static class Functions
    {
        /// <summary>
        /// Two functions composition
        /// </summary>
        public static Func<a, c> Compose<a, b, c>(Func<b, c> g, Func<a, b> f) => v => g(f(v));

        /// <summary>
        /// Three functions composition
        /// </summary>
        public static Func<a, d> Compose<a, b, c, d>(Func<c, d> h, Func<b, c> g, Func<a, b> f) => v => h(g(f(v)));

        /// <summary>
        /// Create a function which ignores input and returns value, evaluated by the agrument function
        /// </summary>
        public static Func<inˈ, outˈ> Const<inˈ, outˈ>(Func<Unit, outˈ> v) => _ => v(Unit());

        /// <summary>
        /// Take the first argument
        /// </summary>
        public static a Fst<a, b>(a va, b vb) => va;
        /// <summary>
        /// Take the second argument
        /// </summary>
        public static b Snd<a, b>(a va, b vb) => vb;

        /// <summary>
        /// Apply the first argument to the second (reified Invoke)
        /// </summary>
        public static b App<a, b>(Func<a, b> f, a v) => f(v);

        /// <summary>
        /// Identity function
        /// </summary>
        public static val Id<val>(val value) => value;

        /// <summary>
        /// Take a function and flip its arguments
        /// </summary>
        public static Func<inB, inA, outˈ> Flip<inA, inB, outˈ>(this Func<inA, inB, outˈ> f) => (a, b) => f(b, a);

        /// <summary>
        /// Currying for two args function
        /// </summary>
        public static Func<inA, Func<inB, outˈ>> Curry<inA, inB, outˈ>(Func<inA, inB, outˈ> f) => a => b => f(a, b);
        /// <summary>
        /// Currying for three args function
        /// </summary>
        public static Func<inA, Func<inB, Func<inC, outˈ>>> Curry<inA, inB, inC, outˈ>(Func<inA, inB, inC, outˈ> f) => a => b => c => f(a, b, c);
        /// <summary>
        /// Currying for four args function
        /// </summary>
        public static Func<inA, Func<inB, Func<inC, Func<inD, outˈ>>>> Curry<inA, inB, inC, inD, outˈ>(Func<inA, inB, inC, inD, outˈ> f) => a => b => c => d => f(a, b, c, d);
        
        /// <summary>
        /// Disambiguation between `Func` and `Expression`
        /// </summary>
        public static Func<a, b> Func<a, b>(this Func<a, b> f) => f;
        /// <summary>
        /// Disambiguation between `Func` and `Expression`
        /// </summary>
        public static Func<a, b, c> Func<a, b, c>(this Func<a, b, c> f) => f;
        /// <summary>
        /// Disambiguation between `Func` and `Expression`
        /// </summary>
        public static Func<a, b, c, d> Func<a, b, c, d>(this Func<a, b, c, d> f) => f;
        
        /// <summary>
        /// Isomorphism between `a` and `Func{a}`
        /// </summary>
        public static Func<Unit, val> ToFunc<val>(this val that) => _ => that;
        /// <summary>
        /// Isomorphism between `Func{a}` and `a`
        /// </summary>
        public static val ToValue<val>(this Func<Unit, val> that) => that(Unit());

        /// <summary>
        /// Isomorphism between `Func{a}` and `Func{Unit, a}`
        /// </summary>
        public static Func<Unit, outˈ> IsoFunc<outˈ>(this Func<outˈ> that) => _ => that();
        /// <summary>
        /// Isomorphism between `Func{Unit, a}` and `Func{a}`
        /// </summary>
        public static Func<outˈ> IsoFunc<outˈ>(this Func<Unit, outˈ> that) => () => that(Unit());
        
        /// <summary>
        /// Isomorphism between `Func{a, _}` and `Action{a}`
        /// </summary>
        public static Action<inˈ> ToAction<inˈ, outˈ>(this Func<inˈ, outˈ> that) => v => that(v);
        /// <summary>
        /// Isomorphism between `Action{a}` and `Func{a, _}`
        /// </summary>
        public static Func<inˈ, Unit> ToFunc<inˈ>(this Action<inˈ> that) => v => { that(v); return Unit(); };

        /// <summary>
        /// Isomorphism between `Func{Unit, _}` and `Action`
        /// </summary>
        public static Action ToAction<outˈ>(this Func<Unit, outˈ> that) => () => that(Unit());
        /// <summary>
        /// Isomorphism between `Action` and `Func{Unit, _}`
        /// </summary>
        public static Func<Unit, Unit> ToFunc(this Action that) => _ => { that(); return Unit(); };

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
        /// Dual to `App`
        /// </summary>
        /// <remarks>
        /// Useful for monadic evaluation chains for nullable types
        /// </remarks>
        /// <example><code>
        /// decimal? d = TryToCalculateOrReturnNothing();
        /// return d
        ///     ?.FeedTo(TransformValueSomehow)
        ///     ?.FeedTo(TransformTheResultInAnotherWay)
        ///     ?.FeedTo(TransformTheResultOneMoreTime);
        /// </code></example>
        public static outˈ FeedTo<inˈ, outˈ>(this inˈ that, Func<inˈ, outˈ> f) => f(that);
        
        /// <summary>
        /// For nullable and constructable type replace null by a new object
        /// </summary>
        public static T NewIfNull<T>(this T that) where T : class, new() => that ?? new T();

        /// <summary>
        /// Finds function fixed point
        /// </summary>
        public static Func<inˈ, outˈ> Fix<inˈ, outˈ>(Func<Func<inˈ, outˈ>, Func<inˈ, outˈ>> f) => v => f(Fix(f))(v);

        /// <summary>
        /// Enforce squential calculation
        /// </summary>
        public static b Seq<a, b>(a _, b value) => value;
    }
}
