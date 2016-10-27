
namespace Dotnet.Func.Ext.Collections
{
    using Algebras;
    using Data;
    using System.Collections.Generic;
    using static Algebras.Structures;
    using static Data.Units;

    public static class Ranges
    {
        /// <summary>
        /// Generate range of ints with step
        /// </summary>
        /// <param name="from">Starting number</param>
        /// <param name="by">Step</param>
        /// <param name="to">The last number in a sequence (or the point of non-crossing)</param>
        public static IEnumerable<e> EnumFromByTo<e, a>(e from, e by, e to, a alg)
            where a : ROrderByCompare<e, Unit>, SMonoid<e, Additive<Unit>>
        {
            if (alg.Equal(by, alg.Zero()))
                while (true)
                    yield return from;

            var direction = alg.Compare(to, from);

            if (direction.IsEq())
                yield return from;

            if (direction != alg.Compare(by, alg.Zero()))
                yield break;

            var current = from;

            while (true)
            {
                if (alg.Compare(to, current) != direction && alg.Equal(current, to).Not())
                    yield break;

                yield return current;

                current = alg.Add(current, by);
            }
        }

        /// <summary>
        /// Generate range of ints by example
        /// </summary>
        /// <param name="from">Starting unmber</param>
        /// <param name="then">The second number</param>
        /// <param name="to">The last number in a sequence (or the point of non-crossing)</param>
        public static IEnumerable<e> EnumFromThenTo<e, a>(e from, e then, e to, a alg) where a : ROrderByCompare<e, Unit>, SGroup<e, Additive<Unit>> =>
            EnumFromByTo(from, alg.Sub(then, from), to, alg);

        /// <summary>
        /// Generate infinite range of ints by step
        /// </summary>
        /// <param name="from">Staring number</param>
        /// <param name="by">Step</param>
        public static IEnumerable<e> EnumFromBy<e, a>(e from, e by, a alg)
             where a : ROrderByCompare<e, Unit>, SSemigroup<e, Additive<Unit>>
        {
            var current = from;

            while (true)
            {
                yield return current;

                current = alg.Add<e, Unit>(current, by);
            }
        }
        
        /// <summary>
        /// Generate infinite range of ints by example
        /// </summary>
        /// <param name="from">Starting number</param>
        /// <param name="then">The second number</param>
        public static IEnumerable<e> EnumFromThen<e, a>(e from, e then, a alg) where a : ROrderByCompare<e, Unit>, SGroup<e, Additive<Unit>> =>
            EnumFromBy(from, alg.Sub(then, from), alg);

        /// <summary>
        /// Generate infinite range of ints with step of one
        /// </summary>
        /// <param name="from">Starting number</param>
        public static IEnumerable<e> EnumFrom<e, a>(e from, a alg) where a : ROrderByCompare<e, Unit>, SRing<e, Unit> => EnumFromBy(from, alg.One(), alg);

        /// <summary>
        /// Generate range of ints with step of one
        /// </summary>
        /// <param name="from">Starting number</param>
        /// <param name="to">The last number in a sequence</param>
        public static IEnumerable<e> EnumFromTo<e, a>(e from, e to, a alg)
            where a : ROrderByCompare<e, Unit>, SRing<e, Unit>
        {
            var direction = alg.Compare(to, from);

            if (direction.IsEq())
            {
                yield return from;
                yield break;
            }

            var increment = direction.Case(alg.MinusOne(), alg.Zero(), alg.One());

            var current = from;

            while (true)
            {
                yield return current;

                if (alg.Compare(to, current) != direction)
                    yield break;

                current = alg.Add(current, increment);
            }
        }
    }
}
