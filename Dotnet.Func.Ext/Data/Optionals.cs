namespace Dotnet.Func.Ext.Data
{
    using Algebraic;
    using Collections;
    using System;
    using System.Collections.Generic;
    using static Algebraic.Signatures;
    using static Ctors;
    using static Exceptions;
    using static Optionals;
    using static Units;
    using static Core.Functions;
    using System.Linq;

    public static partial class Ctors
    {
        /// <summary>
        /// Ctor for empty Opt
        /// </summary>
        public static Opt<val> None<val>() => Opt<val>.CreateNone();
        /// <summary>
        /// Empty opt injection
        /// </summary>
        public static Opt<val> None<val>(Unit _) => Opt<val>.CreateNone();

        public static None None() => new None();
        public static None None(Unit _) => new None();

        /// <summary>
        /// Non-empty opt injection
        /// </summary>
        public static Opt<val> Some<val>(val value) => Opt<val>.CreateSome(value);
        
        /// <summary>
        /// Nullable injection
        /// </summary>
        public static val? Nullable<val>(val value) where val : struct => value;
    }

    public static partial class Dtors
    {
        /// <summary>
        /// Empty opt projection
        /// </summary>
        public static Unit None<val>(this Opt<val> that) =>
            that.Case(Unit(), Snd, Failing<Unit>.Tag(that, nameof(None)), (t, _) => t.ToValue());

        /// <summary>
        /// Non-empty opt projection
        /// </summary>
        public static val Some<val>(this Opt<val> that) =>
            that.Case(Failing<val>.Tag(that, nameof(Some)), (t, _) => t.ToValue(), Unit(), Snd);

        /// <summary>
        /// Empty opt projection test
        /// </summary>
        public static bool IsNone<val>(this Opt<val> that) => that.Case(true, Fst, false, Fst);
        /// <summary>
        /// Non-empty opt projection test
        /// </summary>
        public static bool IsSome<val>(this Opt<val> that) => that.Case(false, Fst, true, Fst);
    }

    public static class Optionals
    {
        /// <summary>
        /// Data container, either stores data of some type or doesn't store anything
        /// Can be recursively contained unlike `Nullable{}`
        /// Defaults to None
        /// Could be viewed as an immutable list with as most one element
        /// 
        /// data Opt val = None | Some val
        /// </summary>
        /// <example>
        /// Safe integral division
        /// <code>
        /// Opt{int} TryDivide(int a, int b) => b == 0 ? None{int}() : Some(a / b);
        /// </code>
        /// </example>
        /// <typeparam name="val">Type to hold. Any type is allowed, no exceptions</typeparam>
        /// <see cref="https://en.wikipedia.org/wiki/Option_type"/>
        public struct Opt<val> : IEither<Unit, val>
        {
            /// <summary>
            /// Stored value (if there is one)
            /// </summary>
            private val _value;
            private bool _isSome;

            /// <summary>
            /// None smart ctor
            /// </summary>
            public static Opt<val> CreateNone() => new Opt<val> { _value = default(val), _isSome = false };
            
            /// <summary>
            /// Some smart ctor
            /// </summary>
            /// <param name="value">Object to hold</param>
            public static Opt<val> CreateSome(val value) => new Opt<val> { _value = value, _isSome = true };
            
            /// <summary>
            /// Basic closure-evading pattern matcher
            /// </summary>
            public res Case<leftCtx, rightCtx, res>(leftCtx leftCtxˈ, Func<leftCtx, Unit, res> Left, rightCtx rightCtxˈ, Func<rightCtx, val, res> Right) =>
                !_isSome ? Left(leftCtxˈ, Unit()) : Right(rightCtxˈ, _value);

            public override string ToString() => !_isSome ? "None()" : $"Some({_value})";

            public static implicit operator Opt<val>(val value) => Some(value);
            public static implicit operator Opt<val>(None _) => None<val>();
        }

        public static class Opt
        {
            /// <summary>
            /// Pointer functor unary operation
            /// </summary>
            public static Opt<val> Pure<val>(val v) => Some(v);
        }

        /// <summary>
        /// Point operation as an extension
        /// </summary>
        public static Opt<val> PureOpt<val>(this val that) => Opt.Pure(that);

        /// <summary>
        /// Unit of Opt to avoid explicit type declaration
        /// </summary>
        public struct None { }
        
        /// <summary>
        /// Guarantees no nulls for reference values
        /// </summary>
        /// <typeparam name="val">Contained reference type</typeparam>
        public struct NotNull<val>
            where val : class
        {
            private readonly val _value;

            public Opt<val> Value => _value.ToOpt();

            public NotNull(val value) { _value = value; }

            public static implicit operator Opt<val>(NotNull<val> notNull) => notNull.Value;
        }

        public static IEnumerable<val> AsEnumerable<val>(this Opt<val> that) =>
            that.Case(_ => Enumerable.Empty<val>(), v => v.YieldOne());

        /// <summary>
        /// `NoNull` smart ctor
        /// </summary>
        public static NotNull<val> Ensure<val>(val value) where val : class => new NotNull<val>(value);

        #region Extension methods

        /// <summary>
        /// Try to extract contained value or invoke default value source function
        /// </summary>
        /// <param name="None">Default value source function</param>
        /// <returns>Contained value or, if empty, value from the source of default values</returns>
        public static val GetValueOr<val>(this Opt<val> that, Func<Unit, val> None) => that.Case(None, Core.Functions.App, Unit(), Snd);

        /// <summary>
        /// Try to extract contained value or use provided default value
        /// </summary>
        /// <param name="Default">Default value</param>
        /// <returns>Contained value or, for empty, provided default value</returns>
        public static val GetValueOr<val>(this Opt<val> that, val Default) => that.Case(Default, Fst, Unit(), Snd);

        /// <summary>
        /// Get value or fail
        /// </summary>
        public static val Get<val>(this Opt<val> that) => that.Some();

        /// <summary>
        /// Functor map: transform the value inside of the container, if any
        /// Enumerable.Select-like transformation
        /// </summary>
        /// <typeparam name="valˈ">New contained type</typeparam>
        /// <param name="f">Transformation function</param>
        /// <returns>Container with/without value been transformed</returns>
        /// <rules>
        /// None.Map(f) → None  { no side effects executed }
        /// Some(a).Map(f) → Some(f(a))
        /// </rules>
        /// <see cref="https://en.wikipedia.org/wiki/Functor"/>
        public static Opt<valˈ> Map<val, valˈ>(this Opt<val> that, Func<val, valˈ> f) =>
            that.Map(f, (fˈ, v) => fˈ(v));

        /// <summary>
        /// Hoist a function into an optional computation context
        /// </summary>
        public static Func<Opt<a>, Opt<b>> Map<a, b>(Func<a, b> f) => v => v.Map(f);

        public static Opt<valˈ> Map<valCtx, val, valˈ>(this Opt<val> that, valCtx ctx, Func<valCtx, val, valˈ> f) =>
            that.Case(
                Unit(), (_, _ˈ) => None(),
                Pair(ctx, f), (rightMap, rightValue) => Some(rightMap.Right()(rightMap.Left(), rightValue)));

        /// <summary>
        /// Lift a function to optional applicative functor context
        /// </summary>
        public static Opt<outˈ> Lift<inA, inB, outˈ>(this Func<inA, inB, outˈ> f, Opt<inA> a, Opt<inB> b) =>
            a.IsSome() && b.IsSome() ? Some(f(a.Some(), b.Some())) : None();

        /// <summary>
        /// Execute the function in optional context
        /// </summary>
        /// <example><code>
        /// var tricky = a => b => c => a * b ^ c;
        /// Opt{int} va = ..
        /// Opt{int} vb = ..
        /// Opt{int} vc = ..
        /// var result = Some(tricky).Ap(va).Ap(vb).Ap(vc);
        /// </code></example>
        /// <see cref="https://en.wikipedia.org/wiki/Applicative_functor"/>
        public static Opt<outˈ> Ap<inA, outˈ>(this Opt<Func<inA, outˈ>> f, Opt<inA> a) =>
            f.IsSome() && a.IsSome() ? Some(f.Some()(a.Some())) : None();

        /// <summary>
        /// Extract contained structural value as Nullable{}
        /// </summary>
        public static val? UnwrapStruct<val>(this Opt<val> that) where val : struct =>
            that.Map(Nullable).GetValueOr(default(val?));

        /// <summary>
        /// Extract contained reference value or return null
        /// </summary>
        public static val UnwrapClass<val>(this Opt<val> that) where val : class =>
            that.GetValueOr(default(val));

        /// <summary>
        /// Convert from Nullable to Opt (null maps to None)
        /// </summary>
        public static Opt<val> ToOpt<val>(this val? that) where val : struct =>
            that == null ? None() : Some(that.Value);

        /// <summary>
        /// Inject reference into Opt (null maps to None)
        /// </summary>
        public static Opt<val> ToOpt<val>(this val that) where val : class =>
            that == null ? None() : Some(that);

        /// <summary>
        /// Try to cast type
        /// Works in similar way to "referenceValue as SomeRefType"
        /// </summary>
        public static Opt<target> As<target>(this object that) where target : class =>
            that == null ? Some<target>(null) : (that as target).ToOpt();

        /// <summary>
        /// If-then expression analogue
        /// </summary>
        public static Opt<target> Then<target>(this bool condition, Func<Unit, target> then) =>
            condition ? Some(then(Unit())) : None();
        //condition.Homo(ABool.Class, AOpt<Unit>.Class).Map(then); // too cool to use here

        /// <summary>
        /// If-then expression analogue
        /// </summary>
        public static Opt<target> Then<target>(this bool condition, target then) =>
            condition ? Some(then) : None();

        /// <summary>
        /// Ensure that contained value (if any) satisfies the predicate or else return None
        /// Enumerable.Where-like filter
        /// </summary>
        public static Opt<val> Filter<val>(this Opt<val> that, Func<val, bool> pred) =>
            that.Map(pred).GetValueOr(false) ? that : None();

        /// <summary>
        /// Monadic join: flatten the container
        /// Reduce the level of optionality by one
        /// </summary>
        /// <typeparam name="val">Contained type</typeparam>
        /// <param name="that">Container to be flatted</param>
        /// <returns>Container flattened by one level or empty container</returns>
        /// <rules>
        /// None.Flatten() → None
        /// Some(None).Flatten() → None
        /// Some(Some(a)).Flatten() → Some(a)
        /// </rules>
        /// <see cref="https://en.wikipedia.org/wiki/Monad_(functional_programming)#The_Maybe_monad"/>
        public static Opt<val> Flatten<val>(this Opt<Opt<val>> that) =>
            that.Case(None<val>, Id);

        /// <summary>
        /// Reduce the level of optionality by one considering contained Nullable type
        /// </summary>
        public static Opt<val> Flatten<val>(this Opt<val?> that) where val : struct => that.FlatMap(ToOpt);

        /// <summary>
        /// Ensure that the value (if any) is not null or else return None
        /// </summary>
        public static Opt<val> Flatten<val>(this Opt<val> that) where val : class => that.FlatMap(ToOpt);

        /// <summary>
        /// Monadic bind: invoke the computation in optional context
        /// </summary>
        /// <rules>
        /// None.FlatMap(_ => None) → None  { no side effects executed }
        /// None.FlatMap(b => Some(f(b))) → None  { no side effects executed }
        /// Some(a).FlatMap(_ => None) → None
        /// Some(a).FlatMap(b => Some(f(b))) → Some(f(a))
        /// </rules>
        /// <see cref="https://en.wikipedia.org/wiki/Monad_(functional_programming)#The_Maybe_monad"/>
        public static Opt<valˈ> FlatMap<val, valˈ>(this Opt<val> that, Func<val, Opt<valˈ>> f) =>
                that.Map(f).Flatten();

        /// <summary>
        /// Distributive law for Opt(T) over Nullable(T)
        /// </summary>
        /// <rules>
        /// None.Dist() → null
        /// Some(null).Dist() → null
        /// Some(Nullable(a)).Dist() → Nullable(Some(a))
        /// </rules>
        public static Opt<val>? Dist<val>(this Opt<val?> that) where val : struct =>
            that.Case(_ => null, WrapToSome);

        private static Opt<val>? WrapToSome<val>(val? v) where val : struct => v?.FeedTo(Some);

        /// <summary>
        /// Distributive law for Nullable(T) over Optional(T)
        /// </summary>
        /// <rules>
        /// null.Dist() → None
        /// Nullable(None).Dist() → None
        /// Nullable(Some(a)).Dist() → Some(Nullable(a))
        /// </rules>
        public static Opt<val?> Dist<val>(this Opt<val>? that) where val : struct =>
            that?.UnwrapStruct() ?? None<val?>();

        /// <summary>
        /// Linq map analogue
        /// </summary>
        public static Opt<outˈ> Select<inˈ, outˈ>(this Opt<inˈ> that, Func<inˈ, outˈ> f) => that.Map(f);
        /// <summary>
        /// Linq bind analogue
        /// </summary>
        public static Opt<outˈˈ> SelectMany<inˈ, outˈ, outˈˈ>(this Opt<inˈ> that, Func<inˈ, Opt<outˈ>> f, Func<inˈ, outˈ, outˈˈ> s) => s.Lift(that, that.FlatMap(f));
        /// <summary>
        /// Linq filter analogue
        /// </summary>
        public static Opt<val> Where<val>(this Opt<val> that, Func<val, bool> p) => that.Filter(p);

        
        /// <summary>
        /// Safely apply a function to two nullable arguments (applicative functor's `f <*> x <*> y`)
        /// </summary>
        /// <typeparam name="inA">Type of the first argument</typeparam>
        /// <typeparam name="inB">Type of the second argument</typeparam>
        /// <typeparam name="outˈ">Return type of the function</typeparam>
        /// <param name="f">The function</param>
        /// <param name="a">First argument</param>
        /// <param name="b">Second argument</param>
        /// <rules>
        /// null.App(x, y) → null
        /// f.App(null, y) → null
        /// f.App(x, null) → null
        /// f.App(x, y) → f(x, y)
        /// </rules>
        public static outˈ App<inA, inB, outˈ>(this Func<inA, inB, outˈ> f, inA a, inB b)
            where inA : class
            where inB : class
        {
            return f == null || a == null || b == null ? default(outˈ) : f(a, b);
        }

        /// <summary>
        /// Inject structural value into a nullable
        /// </summary>
        public static val? ToNullable<val>(this val value) where val : struct => value;

        #endregion
    }
}
