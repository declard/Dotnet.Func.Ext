namespace Dotnet.Func.Ext.Algebras
{
    // Phantom marker types for multiple inheritance dispatch
    public sealed class Additive<mark> { private Additive() { } }
    public sealed class Multiplicative<mark> { private Multiplicative() { } }
    public sealed class Infimum<mark> { private Infimum() { } }
    public sealed class Supremum<mark> { private Supremum() { } }
    public sealed class Equative<mark> { private Equative() { } }
}
