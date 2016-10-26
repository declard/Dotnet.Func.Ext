namespace Dotnet.Func.Ext.Data
{
    public struct Ratio
    {
        public int Numerator { get; private set; }
        public int Denominator { get; private set; }

        public static Ratio Create(int numerator, int denominator = 1) =>
            denominator == 0 ? Exceptions.InvalidOperation<Ratio>("Denominator is zero") : new Ratio
            {
                Numerator = numerator,
                Denominator = denominator,
            }.Normalize();

        public Ratio Normalize()
        {
            var gcd = Gcd(Numerator, Denominator);

            return new Ratio
            {
                Numerator = Numerator / gcd,
                Denominator = Denominator / gcd,
            };
        }

        public static Ratio Zero = new Ratio { Denominator = 1, Numerator = 0 };
        public static Ratio One = new Ratio { Denominator = 1, Numerator = 1 };
        public static Ratio Neg(Ratio t) => new Ratio { Denominator = t.Denominator, Numerator = -t.Numerator };
        public static Ratio Inv(Ratio t) => new Ratio { Denominator = t.Numerator, Numerator = t.Denominator };
        public static Ratio Add(Ratio l, Ratio r) => new Ratio
        {
            Numerator = l.Numerator * r.Denominator + r.Numerator * l.Denominator,
            Denominator = l.Denominator * r.Denominator,
        }.Normalize();

        public static Ratio Mul(Ratio l, Ratio r) => new Ratio
        {
            Numerator = l.Numerator * r.Numerator,
            Denominator = l.Denominator * r.Denominator,
        }.Normalize();

        public static int Gcd(int l, int r)
        {
            return r == 0 ? l : Gcd(r, l % r);
        }
    }
}
