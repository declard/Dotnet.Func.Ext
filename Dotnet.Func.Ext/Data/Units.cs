namespace Dotnet.Func.Ext.Data
{
    using static Units;

    public static partial class Ctors
    {
        /// <summary>
        /// The only Unit ctor
        /// </summary>
        public static Unit Unit() => new Unit();
    }

    public static class Units
    {
        /// <summary>
        /// That's what `void` type should be: the type with the only singleton value
        /// The nice way to use it is to have `Func(Unit)` instead of `Action` in generic code
        /// </summary>
        public struct Unit { }
    }
}
