# `newtype` (Distinct Type Aliases for C#)

![logo, a stylized N with a red and Blue half](https://raw.githubusercontent.com/outfox/newtype/main/logo.svg)

A source generator that creates distinct type aliases with full operator forwarding. Inspired by Haskell's `newtype` and
F#'s type abbreviations. `newtype` works for a healthy number of types - many primitives, structs, records, classes work out of the box.

## Installation

```shell
dotnet add package newtype
````

## Usage

#### Basic: strongly typed `string`-names, `int`-IDs, and `int`-counts
```csharp
using newtype;

[newtype<int>]
public readonly partial struct Pizzas;

[newtype<double>]
public readonly partial struct Fullness;
```

#### Typical: quantities backed by the same data type. 
*For example, forces, velocities, positions, etc. all lose their semantics when expressed as `Vector3`*
```csharp
using System.Numerics;

[newtype<Vector3>]
public readonly partial struct Position;

[newtype<Vector3>]
public readonly partial struct Velocity;

// Now Position and Velocity are distinct types that behave like Vector3
var p = new Position(1, 2, 3);
var v = new Velocity(new Vector3(0.1f, 0, 0)); // may also construct from aliased type

// Static members are forwarded
Console.WriteLine(Position.UnitX);  // (1, 0, 0)
Console.WriteLine(Position.Zero);   // (0, 0, 0)

// Instance properties work
Console.WriteLine(p.X);      // 1
Console.WriteLine(p.Length()); // 3.74...

// Arithmetic operators work (via implicit conversion to/from Vector3)
Position updated = p + v * deltaTime;

// Implicit conversion both ways
Vector3 vec = p;                              // Position → Vector3
Position pos = new Vector3(); // Vector3 → Position
```

Design patterns such as [Entity-Component Systems](https://github.com/outfox/fennecs) benefit greatly from sleek type
forwarding that goes beyond the primitive, identifier aliasing or record-wrapping that C# brings out of the box.

## What Gets Generated

For each `[newtype<T>]` decorated struct, the generator creates:

### Core Members

- `private readonly T _value` - The backing field
- `public T Value { get; }` - Property to access the value
- Constructor from `T`

### Conversions

- `implicit operator AliasType(T value)` - Convert from aliased type
- `implicit operator T(AliasType value)` - Convert to aliased type

### Operators (forwarded from T)

- All binary operators (`+`, `-`, `*`, `/`, `%`, etc.)
- All unary operators (`-`, `+`, `!`, `~`, `++`, `--`)
- Comparison operators (`<`, `>`, `<=`, `>=`)
- Equality operators (`==`, `!=`)

### Static Members (forwarded from T)

- All public static properties (e.g., `Vector3.UnitX` → `Position.UnitX`)
- All public static readonly fields

### Instance Members (forwarded from T)

- All public instance properties (e.g., `.X`, `.Y`, `.Z`)
- All public instance methods (e.g., `.Length()`, `.Normalize()`)

### Interface Implementations

- `IEquatable<AliasType>`
- `IComparable<AliasType>` (if T implements `IComparable<T>`)
- `IFormattable` (if T implements it)

## Design Decisions

### Implicit vs Explicit Conversion

This generator uses **implicit conversion** both ways. This means:

```csharp
Position p = new Vector3(1, 2, 3);  // ✓ Works
Vector3 v = p;                       // ✓ Works
Position p2 = p + velocity;          // ✓ Works (via Vector3 arithmetic)
```

This is intentional for System.Numerics types where you want the arithmetic to "just work". The type safety comes from *
*distinct types at API boundaries**:

```csharp
// Your API enforces type safety
void ApplyForce(Position target, Velocity force) { ... }

// Caller can't mix them up
ApplyForce(position, velocity);  // ✓ Correct
ApplyForce(velocity, position);  // ✗ Compile error!
```

### Alternative: Explicit Conversion

If you want stricter type safety (no implicit mixing in arithmetic), modify the generator to use `explicit` operators
instead. Then:

```csharp
Position p = (Position)new Vector3(1, 2, 3);  // Must be explicit
Position p2 = (Position)(p.Value + v.Value);   // Must unwrap for arithmetic
```

### Zero-Cost Abstraction

The `readonly struct` with `[MethodImpl(MethodImplOptions.AggressiveInlining)]` means the JIT will optimize away the
wrapper entirely in release builds. The generated code has the same performance as using `Vector3` directly.

## Installation

```bash
dotnet add package newtype
```

## Viewing Generated Code

Enable generated file output in your project:

```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
  <CompilerGeneratedFilesOutputPath>$(BaseIntermediateOutputPath)\GeneratedFiles</CompilerGeneratedFilesOutputPath>
</PropertyGroup>
```

Then check `obj/GeneratedFiles/` after building.

## Requirements

- .NET 7+ SDK (for building the generator)
- C# 11+ (for generic attributes `[newtype<T>]`)
- Target framework can be anything supported by your SDK

## License

MIT - do whatever you want with it.

## Extending

You can extend your partial class with needed additional methods and conversions as needed.

### Adding Custom Cross-Type Operations

If you want `Position + Velocity → Position` without going through `Vector3`, you can add partial methods:

```csharp
[newtype<Vector3>]
public readonly partial struct Position
{
    // Add custom cross-type operation
    public static Position operator +(Position p, Velocity v) 
        => new(p.Value + v.Value);
}
```

### Validation

A simple `Validate` partial method pattern if you want runtime validation.

(note: would need generator modification to call this from constructor, but one might consider a factory method)

```csharp
[newtype<Vector3>]
public readonly partial struct Position
{
    partial void Validate()
    {
        if (float.IsNaN(Value.X) || float.IsNaN(Value.Y) || float.IsNaN(Value.Z))
            throw new ArgumentException("Position cannot contain NaN");
    }
}
```

## Known Limitations

1. **Indexers** - Not currently forwarded (would need special handling)
2. **Generic methods** - Instance methods with generic parameters aren't forwarded
3. **ref returns** - Properties/methods with ref returns need special handling
4. **Events** - Static events aren't forwarded
5. **Nested types** - Not handled

These can be added if needed - PRs welcome!
