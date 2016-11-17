namespace Dotnet.Func.Ext.Data
{
    using System;

    public static partial class Ctors
    {
        /// <summary>
        /// Cont injection
        /// </summary>
        public static Continuations.Cont<r, a> Cont<r, a>(Func<Func<a, r>, r> f) => Continuations.Cont<r, a>.Create(f);
    }
    
    /// <summary>
    /// Continuation-related stuff
    /// </summary>
    public static class Continuations
    {
        /// <summary>
        /// Continuation, incapsulates a computation with a hole instead of `return`
        /// </summary>
        /// <typeparam name="r">Result type</typeparam>
        /// <typeparam name="a">Argument for a returning function</typeparam>
        /// <remarks>Emulates consistent state by providing default arg value to a function when not initialized properly</remarks>
        public struct Cont<r, a>
        {
            Func<Func<a, r>, r> _cont;

            /// <summary>
            /// Smart ctor
            /// </summary>
            /// <param name="cont"></param>
            /// <returns></returns>
            public static Cont<r, a> Create(Func<Func<a, r>, r> cont) => new Cont<r, a> { _cont = cont };

            /// <summary>
            /// Basic pattern matcher (runs a computation)
            /// </summary>
            public r Case(Func<a, r> f) => _cont == null ? f(default(a)) : _cont(f);
        }
        
        /// <summary>
        /// Call with current continuation
        /// </summary>
        public static Cont<r, a> CallCC<r, a, b>(Func<Func<a, Cont<r, b>>, Cont<r, a>> f) =>
            Cont<r, a>.Create(k => (f(aˈ => Cont<r, b>.Create(_ => k(aˈ)))).Case(k));
        
        public static class Cont
        {
            /// <summary>
            /// Pointer functor unary operation
            /// </summary>
            public static Cont<r, a> Pure<r, a>(a aˈ) => Cont<r, a>.Create(k => k(aˈ));
        }

        public struct ConstCont<a>
        {
            a _v;

            public ConstCont(a v) { _v = v; }

            public Cont<r, a> With<r>() => Cont.Pure<r, a>(_v);
        }

        /// <summary>
        /// Point operation as an extension
        /// </summary>
        public static ConstCont<a> PureCont<a>(this a that) => new ConstCont<a>(that);

        /// <summary>
        /// Execute continuation when argument type and return types are same
        /// </summary>
        public static r Uncont<r>(this Cont<r, r> that) => that.Case(Functions.Id);

        /// <summary>
        /// Functor `map` operation
        /// </summary>
        public static Cont<r, b> Map<r, a, b>(this Cont<r, a> that, Func<a, b> f) =>
            Cont<r, b>.Create(k => that.Case(aˈ => k(f(aˈ))));

        /// <summary>
        /// Monad `join` operation
        /// </summary>
        public static Cont<r, a> Join<r, a>(this Cont<r, Cont<r, a>> that) =>
            Cont<r, a>.Create(k => that.Case(aˈ => aˈ.Case(k)));

        /// <summary>
        /// Monad `bind` operation
        /// </summary>
        public static Cont<r, b> Bind<r, a, b>(this Cont<r, a> that, Func<a, Cont<r, b>> f) =>
           that.Map(f).Join();

        public static Cont<r, outˈˈ> SelectMany<r, inˈ, outˈ, outˈˈ>(
            this Cont<r, inˈ> that,
            Func<inˈ, Cont<r, outˈ>> f,
            Func<inˈ, outˈ, outˈˈ> s) => Cont<r, outˈˈ>.Create(k => that.Case(a => f(a).Case(b => k(s(a, b)))));

        public static Cont<r, outˈ> Select<r, inˈ, outˈ>(this Cont<r, inˈ> that, Func<inˈ, outˈ> f) => that.Map(f);
    }
}
