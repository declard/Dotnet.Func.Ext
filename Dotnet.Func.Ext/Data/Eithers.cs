namespace Dotnet.Func.Ext.Data
{
    using Algebras;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using static Algebras.Structures;
    using static Ctors;
    using static Eithers;
    using static Exceptions;
    using static Optionals;
    using static Units;
    using static Core.Functions;

    public static partial class Ctors
    {
        /// <summary>
        /// Type inference helper class
        /// </summary>
        /// <typeparam name="t">Another type</typeparam>
        public class Either<t>
        {
            /// <summary>
            /// Left injection
            /// </summary>
            public static Either<left, t> Left<left>(left value) => Either<left, t>.CreateLeft(value);
            /// <summary>
            /// Right injection
            /// </summary>
            public static Either<t, right> Right<right>(right value) => Either<t, right>.CreateRight(value);
        }
    }

    public static partial class Dtors
    {
        /// <summary>
        /// Left projection
        /// </summary>
        public static left Left<left, right>(this Either<left, right> that) =>
            that.Case(Unit(), Snd, Failing<left>.Tag(that, nameof(Left)), (t, _) => t.ToValue());

        /// <summary>
        /// Right projection
        /// </summary>
        public static right Right<left, right>(this Either<left, right> that) =>
            that.Case(Failing<right>.Tag(that, nameof(Right)), (t, _) => t.ToValue(), Unit(), Snd);

        /// <summary>
        /// Left projection test
        /// </summary>
        public static bool IsLeft<left, right>(this Either<left, right> that) => that.Case(true, Fst, false, Fst);
        /// <summary>
        /// Right projection test
        /// </summary>
        public static bool IsRight<left, right>(this Either<left, right> that) => that.Case(false, Fst, true, Fst);
    }

    public static class Eithers
    {
        /// <summary>
        /// Data container, can store data of either one or another type
        /// Scenario example: "validate and transform thing, return transformed thing or error details"
        /// 
        /// The type can have only two meta-states:
        /// either it contains a value of the `left` type or it contains a value of the `right` type
        /// 
        /// The type is not quite symmetric:
        /// 1. it is a monad by it's `right` parameter
        /// 2. by default it constructs as Left containing the default value for the `left` type (for compatibility with other eithers, e.g. Opt and List)
        /// </summary>
        public struct Either<left, right> : IEither<left, right>, IEnumerable<right>
        {
            private left _left;
            private right _right;
            private bool _isRight;

            /// <summary>
            /// Left smart ctor
            /// </summary>
            public static Either<left, right> CreateLeft(left value) =>
                new Either<left, right> { _right = default(right), _left = value, _isRight = false };

            /// <summary>
            /// Right smart ctor
            /// </summary>
            public static Either<left, right> CreateRight(right value) =>
                new Either<left, right> { _right = value, _left = default(left), _isRight = true };
            
            /// <summary>
            /// Basic closure-evading pattern matcher
            /// </summary>
            public res Case<leftCtx, rightCtx, res>(leftCtx leftCtxˈ, Func<leftCtx, left, res> Left, rightCtx rightCtxˈ, Func<rightCtx, right, res> Right) =>
                !_isRight ? Left(leftCtxˈ, this.Left()) : Right(rightCtxˈ, this.Right());

            public override string ToString() => !_isRight ? $"Left({_left})" : $"Right({_right})";

            public IEnumerator<right> GetEnumerator() => this.TryGetRight().GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public static class Either
        {
            /// <summary>
            /// Pointed functor unary operation
            /// </summary>
            public static Either<left, right> Pure<left, right>(right value) => Either<left>.Right(value);
        }

        public struct RightEither<right>
        {
            right _v;

            public RightEither(right v) { _v = v; }

            public Either<left, right> With<left>() => Either.Pure<left, right>(_v);
        }

        /// <summary>
        /// Point operation as an extension
        /// </summary>
        public static RightEither<right> PureEither<right>(this right that) => new RightEither<right>(that);

        /// <summary>
        /// Bifunctor map: transform the left or right inside of the container
        /// </summary>
        /// <typeparam name="left">Old type of contained left</typeparam>
        /// <typeparam name="right">Old type of contained right</typeparam>
        /// <typeparam name="leftˈ">New type of contained left</typeparam>
        /// <typeparam name="rightˈ">New type of contained right</typeparam>
        /// <param name="Left">Left transformation function</param>
        /// <param name="Right">Right transformation function</param>
        /// <returns>Container with left or right been transformed</returns>
        /// <rules>
        /// Left(a).BiMap(f, g) → Left(f(a))
        /// Right(a).BiMap(f, g) → Right(g(a))
        /// </rules>
        public static Either<leftˈ, rightˈ> BiMap<left, right, leftˈ, rightˈ>(this Either<left, right> that, Func<left, leftˈ> Left, Func<right, rightˈ> Right) =>
            that.Case(Left, (leftMap, leftValue) => Either<rightˈ>.Left(leftMap(leftValue)), Right, (rightMap, rightValue) => Either<leftˈ>.Right(rightMap(rightValue)));

        /// <summary>
        /// Bifunctor closure-evading map: transform the left or right inside of the container
        /// </summary>
        /// <typeparam name="leftCtx">Computation context for the left tansformation</typeparam>
        /// <typeparam name="rightCtx">Computation context for the right tansformation</typeparam>
        /// <typeparam name="left">Old type of contained left</typeparam>
        /// <typeparam name="right">Old type of contained right</typeparam>
        /// <typeparam name="leftˈ">New type of contained left</typeparam>
        /// <typeparam name="rightˈ">New type of contained right</typeparam>
        /// <param name="Left">Left transformation function</param>
        /// <param name="Right">Right transformation function</param>
        /// <returns>Container with left or right been transformed</returns>
        /// <rules>
        /// Left(a).BiMap(f, g) → Left(f(a))
        /// Right(a).BiMap(f, g) → Right(g(a))
        /// </rules>
        public static Either<leftˈ, rightˈ> BiMap<leftCtx, rightCtx, left, right, leftˈ, rightˈ>(
                this Either<left, right> that,
                leftCtx LeftCtx,
                Func<leftCtx, left, leftˈ> Left,
                rightCtx RightCtx,
                Func<rightCtx, right, rightˈ> Right) =>
            that.Case(
                Pair(LeftCtx, Left), (leftPair, leftValue) => Either<rightˈ>.Left(leftPair.Right()(leftPair.Left(), leftValue)),
                Pair(RightCtx, Right), (rightPair, rightValue) => Either<leftˈ>.Right(rightPair.Right()(rightPair.Left(), rightValue)));

        /// <summary>
        /// Extract value when left and right have the same type
        /// </summary>
        public static val Get<val>(this Either<val, val> that) =>
            that.Case(Id, Id);

        /// <summary>
        /// Try to extract contained left or invoke right mapping function
        /// </summary>
        /// <param name="Right">Right transformation</param>
        /// <returns>Contained left or transformed right</returns>
        public static left GetLeftOr<left, right>(this Either<left, right> that, Func<right, left> Right) => that.Flip().GetRightOr(Right);
        /// <summary>
        /// Try to extract contained left or get default value
        /// </summary>
        /// <param name="Default">Default value</param>
        /// <returns>Contained left or default value</returns>
        public static left GetLeftOr<left, right>(this Either<left, right> that, left Default) => that.Flip().GetRightOr(Default);
        /// <summary>
        /// Try to extract contained left or invoke right mapping function (closure-evading version)
        /// </summary>
        /// <param name="Right">Right transformation</param>
        /// <param name="RightCtx">Transformation context</param>
        /// <returns>Contained left or transformed right</returns>
        public static left GetLeftOr<rightCtx, left, right>(this Either<left, right> that, rightCtx RightCtx, Func<rightCtx, right, left> Right) => that.Flip().GetRightOr(RightCtx, Right);

        /// <summary>
        /// Try to extract contained right or invoke left mapping function
        /// </summary>
        /// <param name="Left">Left transformation</param>
        /// <returns>Contained right or transformed left</returns>
        public static right GetRightOr<left, right>(this Either<left, right> that, Func<left, right> Left) => that.Case(Left, Id);
        /// <summary>
        /// Try to extract contained right or get default value
        /// </summary>
        /// <param name="Default">Default value</param>
        /// <returns>Contained right or default value</returns>
        public static right GetRightOr<left, right>(this Either<left, right> that, right Default) => that.Case(Default, Fst, Unit(), Snd);
        /// <summary>
        /// Try to extract contained right or invoke left mapping function (closure-evading version)
        /// </summary>
        /// <param name="Left">Left transformation</param>
        /// <param name="LeftCtx">Transformation context</param>
        /// <returns>Contained right or transformed left</returns>
        public static right GetRightOr<leftCtx, left, right>(this Either<left, right> that, leftCtx LeftCtx, Func<leftCtx, left, right> Left) => that.Case(LeftCtx, Left, Unit(), Snd);
        
        /// <summary>
        /// Get left or fail
        /// </summary>
        public static left GetLeft<left, right>(this Either<left, right> that) => that.Left();
        /// <summary>
        /// Get right or fail
        /// </summary>
        public static right GetRight<left, right>(this Either<left, right> that) => that.Right();

        /// <summary>
        /// Functor map: transform the right value inside of the container, if any, leaving left value intact
        /// </summary>
        /// <typeparam name="rightˈ">New contained right</typeparam>
        /// <param name="Right">Transformation function</param>
        /// <returns>Container with right value been transformed or with intact left value</returns>
        public static Either<left, rightˈ> Map<left, right, rightˈ>(this Either<left, right> that, Func<right, rightˈ> Right) =>
            that.BiMap(Id, Right);

        /// <summary>
        /// Functor map: transform the right value inside of the container, if any, leaving left value intact (closure-evading version)
        /// </summary>
        /// <typeparam name="rightˈ">New contained right</typeparam>
        /// <param name="Right">Transformation function</param>
        /// <returns>Container with right value been transformed or with intact left value</returns>
        public static Either<left, rightˈ> Map<rightCtx, left, right, rightˈ>(this Either<left, right> that, rightCtx RightCtx, Func<rightCtx, right, rightˈ> Right) =>
            that.BiMap(Unit(), Snd, RightCtx, Right);

        /// <summary>
        /// Try to get left safely
        /// </summary>
        public static Opt<left> TryGetLeft<left, right>(this Either<left, right> that) =>
            that.Flip().BiMap(that.Alg().Flip(), _ => Unit(), Id, AOpt<left>.Class);
        /// <summary>
        /// Try to get right safely
        /// </summary>
        public static Opt<right> TryGetRight<left, right>(this Either<left, right> that) =>
            that.BiMap(that.Alg(), _ => Unit(), Id, AOpt<right>.Class);

        /// <summary>
        /// Select values by boolean value
        /// </summary>
        public static Either<left, right> ToEither<left, right>(this bool isRight, left False, right True) =>
            isRight ? Either<left>.Right(True) : Either<right>.Left(False);
            //isRight.Homo(ABool.Class, AEither<Unit, Unit>.Class).BiMap(False, Fst, True, Fst); // well, that also works, but is too awesome to use :)

        /// <summary>
        /// Monadic join: flatten the container
        /// </summary>
        /// <param name="that">Container to be flatted</param>
        /// <returns>Contained container or contained left</returns>
        /// <rules>
        /// Left(a).Join() → Left(a)
        /// Right(Left(a)).Join() → Left(a)
        /// Right(Left(a)).Join() → Right(a)
        /// </rules>
        public static Either<left, right> Join<left, right>(this Either<left, Either<left, right>> that) =>
            that.BiMap(Id, GetRight);

        /// <rules>
        /// Left(a).Bind(b => Left(f(b))) → Left(a)  { no side effects executed }
        /// Left(a).Bind(b => Right(f(b))) → Left(a)  { no side effects executed }
        /// Right(a).Bind(b => Left(f(b))) → Left(f(a))
        /// Right(a).Bind(b => Right(f(b))) → Right(f(a))
        /// </rules>
        public static Either<left, rightˈ> Bind<left, right, rightˈ>(this Either<left, right> that, Func<right, Either<left, rightˈ>> f) =>
            that.Map(f).Join();
        
        /// <summary>
        /// Mirrors left and right
        /// </summary>
        public static Either<right, left> Flip<left, right>(this Either<left, right> that) =>
            that.FlipHomo(that.Alg(), that.Alg().Flip());

        /// <summary>
        /// Lift a function to an applicative context for either
        /// </summary>
        public static Either<left, outˈ> Lift<left, inA, inB, outˈ>(this Func<inA, inB, outˈ> f, Either<left, inA> a, Either<left, inB> b) =>
            Either<left>.Right(Curry(f)).Ap(a).Ap(b);

        /// <summary>
        /// Applies a function inside an applicative context (see Applicative functors)
        /// </summary>
        public static Either<left, rightˈ> Ap<left, right, rightˈ>(this Either<left, Func<right, rightˈ>> f, Either<left, right> a) =>
            f.BiMap(Unit(), (_, leftValue) => leftValue, a, (rightCtx, rightValue) => rightCtx.Map(rightValue)).Join();

        /// <summary>
        /// Ensure that contained value (if any) satisfies the predicate or else return Left
        /// Enumerable.Where-like filter
        /// </summary>
        public static Either<left, right> Filter<left, right>(this Either<left, right> that, Func<right, bool> p, Resolver.Resolvable<SNeutral<left, Additive<Unit>>> neutral = null) =>
            that.Map(p).GetRightOr(false) ? that : that.Alg().InjectLeft(neutral.Value().Zero());

        /// <summary>
        /// Linq map analogue
        /// </summary>
        public static Either<left, rightOut> Select<left, right, rightOut>(this Either<left, right> that, Func<right, rightOut> f) => that.Map(f);
        /// <summary>
        /// Linq bind analogue
        /// </summary>
        public static Either<left, rightˈˈ> SelectMany<left, right, rightˈ, rightˈˈ>(this Either<left, right> that, Func<right, Either<left, rightˈ>> f, Func<right, rightˈ, rightˈˈ> s) =>
            s.Lift(that, that.Bind(f));
        /// <summary>
        /// Linq filter analogue
        /// </summary>
        public static Either<left, right> Where<left, right>(this Either<left, right> that, Func<right, bool> p) =>
            that.Filter(p);
    }
}
