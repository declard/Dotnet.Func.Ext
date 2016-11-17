
namespace Dotnet.Func.Ext.Collections
{
    using Algebras;
    using Data;
    using System.Collections.Generic;
    using static Data.Orders;

    public static class Ranges
    {
        /// <summary>
        /// Generate range of es with step
        /// </summary>
        /// <param name="from">Starting element</param>
        /// <param name="by">Step</param>
        /// <param name="to">The last element in a sequence (or the point of non-crossing)</param>
        public static IEnumerable<int> EnumFromByTo(int from, int by, int to)
        {
            var alg = AInt32.Class;

            var direction = alg.Compare(to, from);

            // if no step needed to be made then yield only one element
            if (direction.IsEq())
            {
                yield return from;
                yield break;
            }

            var next = alg.Add(from, by);

            // calculate direction to move
            var stepDirection = alg.Compare(next, from);

            // if step doesn't get us closer to `to` then nothing to enumerate
            if (Ord.Compare(direction, stepDirection).IsNeq())
                yield break;

            var current = from;

            while (true)
            {
                if (Ord.Compare(alg.Compare(to, current), direction).IsNeq() && alg.Equal(current, to).Not())
                    yield break;

                yield return current;

                current = alg.Add(current, by);
            }
        }

        /// <summary>
        /// Generate range of es by example
        /// </summary>
        /// <param name="from">Starting element</param>
        /// <param name="then">The second element</param>
        /// <param name="to">The last element in a sequence (or the point of non-crossing)</param>
        public static IEnumerable<int> EnumFromThenTo(int from, int then, int to)
        {
            return EnumFromByTo(from, then - from, to);
        }

        /// <summary>
        /// Generate infinite range of es by step
        /// </summary>
        /// <param name="from">Staring element</param>
        /// <param name="by">Step</param>
        public static IEnumerable<int> EnumFromBy(int from, int by)
        {
            var current = from;

            var direction = AInt32.Class.Compare(by, 0);

            while (true)
            {
                yield return current;

                current += by;

                if (Ord.Compare(AInt32.Class.Compare(current, from), direction).IsNeq())
                    yield break;
            }
        }

        /// <summary>
        /// Generate infinite range of es by example
        /// </summary>
        /// <param name="from">Starting element</param>
        /// <param name="then">The second element</param>
        public static IEnumerable<int> EnumFromThen(int from, int then) =>
            EnumFromBy(from, then - from);

        /// <summary>
        /// Generate infinite range of es with step of one
        /// </summary>
        /// <param name="from">Starting element</param>
        public static IEnumerable<int> EnumFrom(int from) => EnumFromBy(from, 1);

        /// <summary>
        /// Generate range of es with step of one
        /// </summary>
        /// <param name="from">Starting element</param>
        /// <param name="to">The last element in a sequence</param>
        public static IEnumerable<int> EnumFromTo(int from, int to)
        {
            var alg = AInt32.Class;

            var direction = alg.Compare(to, from);

            if (direction.IsEq())
            {
                yield return from;
                yield break;
            }

            var increment = direction.Case(-1, 0, 1);

            var current = from;

            while (true)
            {
                yield return current;

                if (Ord.Compare(alg.Compare(to, current), direction).IsNeq())
                    yield break;

                current += increment;
            }
        }
    }
}
