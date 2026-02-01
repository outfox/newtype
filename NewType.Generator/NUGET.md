# `newtype` *(Distinct Type Aliases for C#)*

![logo, a stylized N with a red and blue half](https://raw.githubusercontent.com/outfox/newtype/main/logo.svg)

A source generator that creates **distinct type aliases** with full operator, constructor, and member forwarding. Inspired by Haskell's `newtype`. Works with primitives, structs, records, and classes.

```shell
dotnet add package newtype
```

## Quick Start

```csharp
using newtype;

// Typed IDs — no more mixing up int parameters
[newtype<int>]
public readonly partial struct PlayerId;

// Domain quantities — Position and Velocity can't be accidentally swapped
[newtype<Vector3>]
public readonly partial struct Position;

[newtype<Vector3>]
public readonly partial struct Velocity;

// Rich string aliases — all string operations forwarded
[newtype<string>]
public readonly partial struct Email;

// Also works with records, classes, and record classes
[newtype<double>]
public partial record struct Duration;  // gets compiler-synthesized equality

[newtype<string>]
public partial class DisplayName;       // reference-type wrapper with null safety
```

Everything works as you'd expect:

```csharp
var id = new PlayerId(42);
PlayerId next = id + 1;                    // arithmetic forwarded

var pos = new Position(1, 2, 3);           // constructor forwarded from Vector3
Console.WriteLine(pos.X);                  // instance members forwarded
Position origin = Position.Zero;           // static members forwarded

Email addr = "alice@example.com";          // implicit conversion from string
string raw = addr;                         // implicit conversion to string
Email greeting = addr + " (verified)";     // string concatenation works

void Move(Position p, Velocity v) { ... }
Move(velocity, position);                  // compile error — type safety!
```

## What Gets Generated

For each `[newtype<T>]` partial type, the generator emits:

| Category | What |
|---|---|
| **Core** | Backing field, `Value` property, constructor from `T`, forwarded constructors |
| **Conversions** | Implicit operators both ways (`T` <-> alias) |
| **Operators** | Binary (`+` `-` `*` `/` `%` `&` `\|` `^` `<<` `>>`), unary (`-` `+` `!` `~` `++` `--`), comparison, equality |
| **Static members** | Properties and readonly fields (e.g. `Vector3.UnitX` -> `Position.UnitX`) |
| **Instance members** | Properties and methods (e.g. `.X`, `.Length()`) |
| **Interfaces** | `IEquatable<T>`, `IComparable<T>`, `IFormattable` (when the aliased type implements them) |

All generated members use `[MethodImpl(MethodImplOptions.AggressiveInlining)]` by default for zero-cost abstraction.

## Constraining Generation

Use `NewtypeOptions` to suppress features, and `MethodImpl` to control inlining:

```csharp
// No implicit int -> PlayerId (must use constructor)
[newtype<int>(Options = NewtypeOptions.NoImplicitWrap)]
public readonly partial struct PlayerId;

// No implicit conversions either way
[newtype<int>(Options = NewtypeOptions.NoImplicitConversions)]
public readonly partial struct OpaqueId;

// Fully locked down — no conversions, no forwarded constructors
[newtype<Vector3>(Options = NewtypeOptions.Opaque)]
public readonly partial struct StrictPosition;

// Let the JIT decide about inlining (omit [MethodImpl] entirely)
[newtype<int>(MethodImpl = default)]
public readonly partial struct RelaxedId;
```

### `NewtypeOptions` Flags

| Flag | Effect |
|---|---|
| `None` | All features enabled *(default)* |
| `NoImplicitWrap` | Suppress `T` -> alias implicit conversion |
| `NoImplicitUnwrap` | Suppress alias -> `T` implicit conversion |
| `NoConstructorForwarding` | Suppress forwarded constructors from `T` |
| `NoImplicitConversions` | `NoImplicitWrap \| NoImplicitUnwrap` |
| `Opaque` | `NoImplicitConversions \| NoConstructorForwarding` |

With any option, the primary constructor (`new Alias(T value)`) and `.Value` property are always available.

## Extending Your Types

Since the alias is a `partial` type, you can add custom members:

```csharp
[newtype<Vector3>]
public readonly partial struct Position
{
    public static Position operator +(Position p, Velocity v)
        => new(p.Value + v.Value);
}
```

## Viewing Generated Code

Enable generated file output in your project:

```xml
<PropertyGroup>
  <EmitCompilerGeneratedFiles>true</EmitCompilerGeneratedFiles>
</PropertyGroup>
```

Then inspect `obj/Debug/net10.0/GeneratedFiles/` after building.

## Requirements

- .NET 8+ SDK
- C# 11+ (for generic attributes `[newtype<T>]`; a `[newtype(typeof(T))]` fallback exists for older versions)

## Known Limitations

Indexers, generic instance methods, ref returns, events, and nested types are not forwarded. PRs welcome!

## License

MIT
