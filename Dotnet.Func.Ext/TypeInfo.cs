namespace Dotnet.Func.Ext
{
    public static class TypeInfo<T>
    {
        public static readonly bool IsValueType;

        static TypeInfo()
        {
            IsValueType = typeof(T).IsValueType;
        }
    }
}
