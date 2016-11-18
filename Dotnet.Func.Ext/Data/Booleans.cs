namespace Dotnet.Func.Ext.Data
{
    using System;
    using static Units;
    using static Functions;
    using static Exceptions;

    public static partial class Ctors
    {
        /// <summary>
        /// False ctor
        /// </summary>
        public static bool False() => false;
        /// <summary>
        /// False injection
        /// </summary>
        public static bool False(Unit _) => false;
        /// <summary>
        /// True ctor
        /// </summary>
        public static bool True() => true;
        /// <summary>
        /// True injection
        /// </summary>
        public static bool True(Unit _) => true;
    }

    public static partial class Dtors
    {
        /// <summary>
        /// False projection
        /// </summary>
        public static Unit False(this bool that) => !that ? Ctors.Unit() : Fail<Unit>.Tag(that, nameof(False));
        /// <summary>
        /// True projection
        /// </summary>
        public static Unit True(this bool that) => that ? Ctors.Unit() : Fail<Unit>.Tag(that, nameof(True));
    }

    /// <summary>
    /// Functional extensions for the `bool` type
    /// </summary>
    public static class Booleans
    {
        /// <summary>
        /// Basic pattern matcher
        /// </summary>
        public static outˈ Case<outˈ>(this bool boolˈ, Func<Unit, outˈ> falseˈ, Func<Unit, outˈ> trueˈ) => boolˈ.Case(falseˈ, Fst, trueˈ, Fst)(Ctors.Unit());

        /// <summary>
        /// Basic closure-evading pattern matcher
        /// </summary>
        public static outˈ Case<falseCtx, trueCtx, outˈ>(this bool boolˈ, falseCtx falseCtxˈ, Func<falseCtx, Unit, outˈ> falseˈ, trueCtx trueCtxˈ, Func<trueCtx, Unit, outˈ> trueˈ) =>
            boolˈ ? falseˈ(falseCtxˈ, Ctors.Unit()) : trueˈ(trueCtxˈ, Ctors.Unit());

        /// <summary>
        /// Boolean inversion
        /// </summary>
        public static bool Not(this bool that) => !that;

        /// <summary>
        /// if that then value else default
        /// </summary>
        public static val ThenOrDefault<val>(this bool that, val value) => that.Case(default(val), Fst, value, Fst);

        /// <summary>
        /// Closure-evading `Then`
        /// </summary>
        public static outˈ ThenOrDefault<inCtx, inˈ, outˈ>(this bool that, inCtx ctx, Func<inCtx, Unit, outˈ> f) => that.Case(default(outˈ), Fst, ctx, f);

        /// <summary>
        /// Ternary operator analogue for functions applications
        /// </summary>
        public static outˈ CaseFeedTo<inˈ, outˈ>(this inˈ that, bool cond, Func<inˈ, outˈ> onFalse, Func<inˈ, outˈ> onTrue) =>
            cond ? onTrue(that) : onFalse(that);

        /// <summary>
        /// Ternary operator analogue for functions applications (predicate version)
        /// </summary>
        public static outˈ CaseFeedTo<inˈ, outˈ>(this inˈ that, Func<inˈ, bool> pred, Func<inˈ, outˈ> onFalse, Func<inˈ, outˈ> onTrue) =>
            pred(that) ? onTrue(that) : onFalse(that);

        /// <summary>
        /// Ternary operator analogue mixed with `Then`
        /// </summary>
        public static val ThenFeedTo<val>(this val that, bool cond, Func<val, val> onTrue) =>
            that.CaseFeedTo(cond, onFalse: Id, onTrue: onTrue);

        /// <summary>
        /// Ternary operator analogue mixed with `Then` (predicate version)
        /// </summary>
        public static val ThenFeedTo<val>(this val that, Func<val, bool> pred, Func<val, val> onTrue) =>
            that.CaseFeedTo(pred, onFalse: Id, onTrue: onTrue);
    }
}
