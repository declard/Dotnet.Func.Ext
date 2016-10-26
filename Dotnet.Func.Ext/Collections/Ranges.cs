
namespace Dotnet.Func.Ext.Collections
{
    using System;
    using System.Collections.Generic;

    public static class Ranges
    {
        /// <summary>
        /// Generate range of ints with step
        /// </summary>
        /// <param name="from">Starting number</param>
        /// <param name="by">Step</param>
        /// <param name="to">The last number in a sequence (or the point of non-crossing)</param>
        public static IEnumerable<int> EnumFromByTo(int from, int by, int to)
        {
            if (by == 0)
                while (true)
                    yield return from;

            var direction = Math.Sign(to - from);

            if (direction == 0)
                yield return from;

            if (direction != Math.Sign(by))
                yield break;

            var current = from;

            while (true)
            {
                if (Math.Sign(to - current) != direction && current != to)
                    yield break;

                yield return current;

                current += by;
            }
        }

        /// <summary>
        /// Generate range of ints by example
        /// </summary>
        /// <param name="from">Starting unmber</param>
        /// <param name="then">The second number</param>
        /// <param name="to">The last number in a sequence (or the point of non-crossing)</param>
        public static IEnumerable<int> EnumFromThenTo(int from, int then, int to) =>
            EnumFromByTo(from, then - from, to);

        /// <summary>
        /// Generate infinite range of ints by step
        /// </summary>
        /// <param name="from">Staring number</param>
        /// <param name="by">Step</param>
        public static IEnumerable<int> EnumFromBy(int from, int by)
        {
            int current = from;

            while (true)
            {
                yield return current;

                current += by;
            }
        }
        
        /// <summary>
        /// Generate infinite range of ints by example
        /// </summary>
        /// <param name="from">Starting number</param>
        /// <param name="then">The second number</param>
        public static IEnumerable<int> EnumFromThen(int from, int then) =>
            EnumFromBy(from, then - from);

        /// <summary>
        /// Generate infinite range of ints with step of one
        /// </summary>
        /// <param name="from">Starting number</param>
        public static IEnumerable<int> EnumFrom(int from) => EnumFromBy(from, 1);

        /// <summary>
        /// Generate range of ints with step of one
        /// </summary>
        /// <param name="from">Starting number</param>
        /// <param name="to">The last number in a sequence</param>
        public static IEnumerable<int> EnumFromTo(int from, int to)
        {
            var direction = Math.Sign(to - from);

            if (direction == 0)
            {
                yield return from;
                yield break;
            }

            var current = from;

            while (true)
            {
                yield return current;

                if (Math.Sign(to - current) != direction)
                    yield break;

                current += direction;
            }
        }
    }
}
