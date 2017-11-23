namespace Dotnet.Func.Ext
{
    using System;
    using System.Collections;

    public struct Hasher : IEnumerable
    {
        public int? Hash { get; private set; }

        public void Add<T>(T value)
        {
            if (TypeInfo<T>.IsValueType)
                AddImpl(value.GetHashCode()); // value is never null
            else
                AddImpl(value?.GetHashCode()); // only reference types get here
        }

        public void Add<T>(T? value) where T : struct
        {
            AddImpl(value?.GetHashCode()); // Nullable<> get here
        }

        private void AddImpl(int? value)
        {
            Hash = Combine(Hash, value);
        }

        public override int GetHashCode() => Hash ?? 17;
        public static implicit operator int(Hasher h) => h.GetHashCode();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        public static int Combine(int? left, int? right)
        {
            unchecked { return 37 * (left ?? 17) + (right ?? 0); }
        }

        public Hasher Combine(int? value) => new Hasher { Hash = Combine(Hash, value) };
    }
}
