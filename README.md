# Functional extensions and data library for dotnet

Conventions:
- Primitive types are structs
- There are no primitive type states but their main states (no states like "uninitialized")
- Primitive type has constructors and an eliminator which combine into identity functions
- Summands in sum-types are ordered ascending by their set power (i.e. Unit < Int < Int * Int < a < Int * a < a * a)
- The leftmost summand of a container is the default one in case the type is initialized to default

The primitive types are:
- Unit = U
- Identity a = Identity a
- Opt a = None | Some a
- Either l r = Left l | Right r
- List a = Nil | Cons a (List a)
- Ord = Lt | Eq | Gt
- Cont r a = Cont ((a -> r) -> r)

There are typeclass-like structures defined for the following types:
- Int32 (Order, Ring, Bounded, Enum, UnitInjection)
- Char (Order, Bounded, Group<Additive>, Neutral<Multiplicative>, Enum, UnitInjection)
- Double (Field, Order, UnitInjection)
- Bool (Order, Bounded SumProjection, SumInjection, Enum)
- String (Equality, Monoid<Additive>, List, UnitInjection)
- Ord (Order, Bounded, Enum)
- Either<,> (SumProjection, SumInjection, UnitInjection, Eq(over Eq), Ord(over Ord))
- Opt<> (SumProjection, SumInjection, UnitInjection, Monoid(left), Monoid(right), Monoid(over Semigroup), Eq(over Eq), Ord(over Ord))
- List<> (Monoid<Additive>, UnitInjection, List)
- Object (Neutral)

TBD
