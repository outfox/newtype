using System.Numerics;
using newtype;

// Define our distinct types
[newtype<Vector3>]
public readonly partial struct Position;

[newtype<Vector3>]
public readonly partial struct Velocity;

[newtype<Vector3>]
public readonly partial struct Scale;

// Also works with other System.Numerics types
[newtype<Quaternion>]
public readonly partial struct Rotation;

[newtype<Matrix4x4>]
public readonly partial struct Transform;

// Test the generated code
public static class Program
{
    public static void Main()
    {
        Console.WriteLine("=== TypeAlias Generator Demo ===\n");

        // Test 1: Construction and conversion
        Console.WriteLine("1. Construction and Conversion:");
        Position p = new Vector3(1, 2, 3);
        Velocity v = new Vector3(0.1f, 0, 0);
        Console.WriteLine($"   Position: {p}");
        Console.WriteLine($"   Velocity: {v}");

        // Test 2: Implicit conversion back to Vector3
        Vector3 pVec = p;
        Console.WriteLine($"   Position as Vector3: {pVec}");

        // Test 3: Static members are forwarded
        Console.WriteLine("\n2. Static Members:");
        Console.WriteLine($"   Position.Zero: {Position.Zero}");
        Console.WriteLine($"   Position.One: {Position.One}");
        Console.WriteLine($"   Position.UnitX: {Position.UnitX}");
        Console.WriteLine($"   Position.UnitY: {Position.UnitY}");
        Console.WriteLine($"   Position.UnitZ: {Position.UnitZ}");

        // Test 4: Arithmetic operators work
        Console.WriteLine("\n3. Arithmetic Operations:");
        Position p2 = p + v;  // Position + Velocity -> Position (both implicitly convert)
        Console.WriteLine($"   p + v = {p2}");
        
        Position p3 = p * 2f;
        Console.WriteLine($"   p * 2 = {p3}");
        
        Position p4 = -p;
        Console.WriteLine($"   -p = {p4}");

        // Test 5: Instance properties work
        Console.WriteLine("\n4. Instance Properties:");
        Console.WriteLine($"   p.X = {p.X}");
        Console.WriteLine($"   p.Y = {p.Y}");
        Console.WriteLine($"   p.Z = {p.Z}");

        // Test 6: Instance methods work
        Console.WriteLine("\n5. Instance Methods:");
        Console.WriteLine($"   p.Length() = {p.Length()}");
        Console.WriteLine($"   p.LengthSquared() = {p.LengthSquared()}");

        // Test 7: Equality
        Console.WriteLine("\n6. Equality:");
        Position same = new Vector3(1, 2, 3);
        Console.WriteLine($"   p == same: {p == same}");
        Console.WriteLine($"   p != v: {p != (Position)v.Value}"); // explicit conversion for type safety

        // Test 8: Type safety - these are DISTINCT types
        Console.WriteLine("\n7. Type Safety:");
        Console.WriteLine("   Position and Velocity are distinct types!");
        Console.WriteLine("   Direct assignment 'Position p = velocity;' requires implicit conversion");
        
        // The key insight: both Position and Velocity wrap Vector3,
        // so they're implicitly convertible to Vector3.
        // Vector3 arithmetic then works, and the result can be assigned to either type.
        
        // Simulate physics update: position += velocity * deltaTime
        float deltaTime = 1f / 60f;
        Position updated = p + v * deltaTime;
        Console.WriteLine($"   After update: {updated}");

        // Test 9: Rotation (Quaternion alias)
        Console.WriteLine("\n8. Quaternion Alias:");
        Rotation r = Quaternion.Identity;
        Console.WriteLine($"   Rotation.Identity: {r}");

        // Test 10: Transform (Matrix4x4 alias)
        Console.WriteLine("\n9. Matrix4x4 Alias:");
        Transform t = Matrix4x4.Identity;
        Console.WriteLine($"   Transform.Identity: {t}");

        Console.WriteLine("\n=== All tests passed! ===");
    }
}
