namespace Dotnet.Func.Ext
{
    using Collections;
    using Data;
    using System;
    using System.Linq;
    using static Data.Ctors;
    using static Data.Eithers;
    using static Data.Lists;
    using static Data.Optionals;
    using static Data.Units;

    /// <summary>
    /// Exceptions helpers and the safe execution context
    /// </summary>
    public static class Exceptions
    {
        /// <example><code>
        /// string value = shouldNotBeNull ?? Throw{string}(new BillionDollarException())
        /// </code></example>
        public static outˈ Throw<outˈ>(Exception e)
        {
            throw e;
        }

        /// <example><code>
        /// decimal value = shouldNotBeNull.GetValueOr(Throwing{decimal}(new IrrecoverableException())
        /// </code></example>
        public static Func<Unit, outˈ> Throwing<outˈ>(Exception e) => _ => Throw<outˈ>(e);
        /// <example><code>
        /// decimal value = shouldNotBeNull.GetValueOr(Throwing{decimal}(_ => new IrrecoverableException())
        /// </code></example>
        public static Func<Unit, outˈ> Throwing<outˈ>(Func<Unit, Exception> e) => _ => Throw<outˈ>(e(Unit()));

        /// <summary>
        /// Try to safely (by particular kind of exception) run a computation
        /// </summary>
        public static Either<ex, res> Try<res, ex, arg>(Func<arg, res> f, arg a)
            where ex : Exception
        {
            try
            {
                return Either<ex>.Right(f(a));
            }
            catch (ex e)
            {
                return Either<res>.Left(e);
            }
        }
        /// <summary>
        /// Try to safely (by particular kind of exception) run a nullary computation
        /// </summary>
        public static Either<ex, res> Try<res, ex>(Func<Unit, res> f) where ex : Exception => Try<res, ex, Unit>(f, Unit());

        /// <summary>
        /// Catch a particular kind of exception during a computation run
        /// </summary>
        public static res Catching<res, ex, arg>(Func<arg, res> f, arg a, Func<ex, res> c) where ex : Exception => Try<res, ex, arg>(f, a).GetRightOr(c);
        /// <summary>
        /// Catch a particular kind of exception during a nullary computation run
        /// </summary>
        public static res Catching<res, ex>(Func<Unit, res> f, Func<ex, res> c) where ex : Exception => Catching(f, Unit(), c);

        /// <summary>
        /// Run a computation with finalization
        /// </summary>
        public static res Finally<res, arg>(Func<arg, res> f, arg a, Func<Unit, Unit> fin)
        {
            try
            {
                return f(a);
            }
            finally
            {
                fin(Unit());
            }
        }
        /// <summary>
        /// Run a nullary computation with finalization
        /// </summary>
        public static res Finally<res>(Func<Unit, res> f, Func<Unit, Unit> fin) => Finally(f, Unit(), fin);

        /// <summary>
        /// Build a context for safe computation execution
        /// </summary>
        /// <example><code>
        /// Trying(LoadDbRecords).Catching((DbConnectionEception e) => )
        /// </code></example>
        public static TryContext<res, arg> Trying<res, arg>(Func<arg, res> f) => new TryContext<res, arg>(f);

        /// <summary>
        /// Context for safe computation execution
        /// </summary>
        /// <typeparam name="res">Computation result type</typeparam>
        /// <typeparam name="arg">Computation argument type</typeparam>
        public class TryContext<res, arg>
        {
            #region Guts

            private abstract class Catcher
            {
                public abstract Opt<res> Catch(Exception ex);
            }

            private sealed class Catcher<ex> : Catcher
                where ex : Exception
            {
                public Func<ex, Opt<res>> Handler;

                public override Opt<res> Catch(Exception ex) => ex.As<ex>().Bind(Handler);
            }

            private readonly Func<arg, res> _tryed;
            private readonly List<Catcher> _catchers;
            private readonly List<Func<Unit, Unit>> _finalizers;

            private TryContext(Func<arg, res> tryed, List<Catcher> catchers, List<Func<Unit, Unit>> finalizers)
            {
                _tryed = tryed;
                _catchers = catchers;
                _finalizers = finalizers;
            }

            #endregion

            public TryContext(Func<arg, res> tryed)
            {
                _tryed = tryed;
            }
            
            /// <summary>
            /// Add exception handler which may reject handling (kind of filtered exceptions)
            /// </summary>
            public TryContext<res, arg> Catch<ex>(Func<ex, Opt<res>> f) where ex : Exception =>
                new TryContext<res, arg>(_tryed, _catchers.Cons(new Catcher<ex> { Handler = f }), _finalizers);

            /// <summary>
            /// Add exception handler
            /// </summary>
            public TryContext<res, arg> Catch<ex>(Func<ex, res> f) where ex : Exception =>
                new TryContext<res, arg>(_tryed, _catchers.Cons(new Catcher<ex> { Handler = f.Map(Some) }), _finalizers);

            /// <summary>
            /// Add finalizer
            /// </summary>
            public TryContext<res, arg> Finally(Func<Unit, Unit> f) =>
                new TryContext<res, arg>(_tryed, _catchers, _finalizers.Cons(f));

            /// <summary>
            /// Execute computation
            /// </summary>
            public res Go(arg a)
            {
                try
                {
                    return _tryed(a);
                }
                catch(Exception ex)
                {
                    var res = _catchers.Reverse().Select(catcher => catcher.Catch(ex)).CollectSome().TryGetFirst();

                    if (res.IsSome())
                        return res.Some();

                    throw;
                }
                finally
                {
                    _finalizers.Reverse().Foreach(finalizer => finalizer(Unit()));
                }
            }
        }

        /// <summary>
        /// Run nullary computation in a safe context
        /// </summary>
        public static res Go<res>(this TryContext<res, Unit> that) => that.Go(Unit());

        /// <summary>
        /// Enum value is out of supported range
        /// </summary>
        public class EnumOutOfRangeException : Exception
        {
            private EnumOutOfRangeException(string ex) : base(ex) { }

            public static EnumOutOfRangeException Create<enumˈ>(enumˈ value) where enumˈ : struct =>
                new EnumOutOfRangeException($"{typeof(enumˈ).Name}.{value}");
        }

        /// <summary>
        /// Trying to access unavailable union variant
        /// </summary>
        public class VariantException : Exception
        {
            private VariantException(string ex) : base(ex) { }

            public static VariantException Create<structˈ>(structˈ union, string tag) where structˈ : struct =>
                new VariantException($"{union} is not a {typeof(structˈ).Name}.{tag}");
        }

        /// <summary>
        /// Helper class for failures
        /// </summary>
        public static class Fail<res>
        {
            /// <summary>
            /// Projection failure
            /// </summary>
            public static res Tag<structˈ>(structˈ union, string tag) where structˈ : struct =>
                Throw<res>(VariantException.Create(union, tag));
        }

        /// <summary>
        /// Helper class for lazy failures
        /// </summary>
        public static class Failing<res>
        {
            /// <summary>
            /// Lazy projection failure
            /// </summary>
            public static Func<Unit, res> Tag<structˈ>(structˈ union, string tag) where structˈ : struct =>
                Throwing<res>(VariantException.Create(union, tag));
        }

        /// <summary> Fast exception expression ctor </summary>
        public static outˈ NotImplemented<outˈ>(Unit _) => Throw<outˈ>(new NotImplementedException());

        /// <summary> Fast exception expression ctor </summary>
        public static outˈ InvalidOperation<outˈ>(Unit _) => Throw<outˈ>(new InvalidOperationException());

        /// <summary> Fast exception expression ctor </summary>
        public static outˈ InvalidOperation<outˈ>(string message) => Throw<outˈ>(new InvalidOperationException(message));

        /// <summary> Fast exception expression ctor </summary>
        public static outˈ NullReference<outˈ>(Unit _) => Throw<outˈ>(new NullReferenceException());

        /// <summary> Fast exception expression ctor </summary>
        public static outˈ OutOfRange<outˈ>(Unit _) => Throw<outˈ>(new IndexOutOfRangeException());
    }
}
