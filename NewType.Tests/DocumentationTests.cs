using System.Numerics;
using System.Reflection;
using Xunit;

namespace newtype.tests;

/// <summary>
/// Validates every code example in README.md / NUGET.md.
/// If a test here breaks, the documentation is lying.
/// </summary>
public class DocumentationTests
{
    // ── Quick Start: type declarations compile ──

    [Fact]
    public void PlayerId_Constructor()
    {
        var id = new PlayerId(42);
        Assert.Equal(42, id.Value);
    }

    [Fact]
    public void PlayerId_ArithmeticForwarded()
    {
        var id = new PlayerId(42);
        PlayerId next = id + 1;
        Assert.Equal(43, next.Value);
    }

    [Fact]
    public void Position_ForwardedConstructor()
    {
        var pos = new Position(1, 2, 3);
        Assert.Equal(1, pos.X);
        Assert.Equal(2, pos.Y);
        Assert.Equal(3, pos.Z);
    }

    [Fact]
    public void Position_InstanceMembersForwarded()
    {
        var pos = new Position(1, 2, 3);
        Assert.Equal(1, pos.X);
        float len = pos.Length();
        Assert.True(len > 3.7f && len < 3.8f);
    }

    [Fact]
    public void Position_StaticMembersForwarded()
    {
        Position origin = Position.Zero;
        Assert.Equal(new Vector3(0, 0, 0), origin.Value);

        Position unitX = Position.UnitX;
        Assert.Equal(new Vector3(1, 0, 0), unitX.Value);
    }

    [Fact]
    public void Email_ImplicitConversionFromString()
    {
        Email addr = "alice@example.com";
        Assert.Equal("alice@example.com", addr.Value);
    }

    [Fact]
    public void Email_ImplicitConversionToString()
    {
        Email addr = "alice@example.com";
        string raw = addr;
        Assert.Equal("alice@example.com", raw);
    }

    [Fact]
    public void Email_StringConcatenation()
    {
        Email addr = "alice@example.com";
        Email greeting = addr + " (verified)";
        Assert.Equal("alice@example.com (verified)", greeting.Value);
    }

    [Fact]
    public void TypeSafety_PositionAndVelocityAreDistinct()
    {
        // Position and Velocity should not be assignable to each other
        Assert.False(typeof(Position).IsAssignableFrom(typeof(Velocity)));
        Assert.False(typeof(Velocity).IsAssignableFrom(typeof(Position)));
    }

    // ── Quick Start: record struct and class variants ──

    [Fact]
    public void Duration_RecordStruct()
    {
        var d = new Duration(1.5);
        Assert.Equal(1.5, d.Value);

        // record structs get compiler-synthesized equality
        var d2 = new Duration(1.5);
        Assert.Equal(d, d2);
    }

    [Fact]
    public void DisplayName_ClassWrapper()
    {
        var name = new DisplayName("Alice");
        Assert.Equal("Alice", name.Value);
    }

    [Fact]
    public void DisplayName_ImplicitConversions()
    {
        DisplayName name = "Bob";
        string raw = name;
        Assert.Equal("Bob", raw);
    }

    // ── Constraining Generation examples ──

    [Fact]
    public void NoImplicitWrap_MustUseConstructor()
    {
        // StrictEntityId has NoImplicitWrap — no int -> StrictEntityId conversion
        var ops = typeof(StrictEntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.ReturnType == typeof(StrictEntityId));
        Assert.Empty(ops);

        // But constructor works
        var id = new StrictEntityId(42);
        Assert.Equal(42, id.Value);
    }

    [Fact]
    public void NoImplicitConversions_NoImplicitOperators()
    {
        var ops = typeof(OpaqueId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit");
        Assert.Empty(ops);

        // Constructor and Value still work
        var id = new OpaqueId(7);
        Assert.Equal(7, id.Value);
    }

    [Fact]
    public void Opaque_NoConversionsNoForwardedConstructors()
    {
        // StrictPosition has Opaque — no implicit conversions
        var ops = typeof(StrictPosition)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit");
        Assert.Empty(ops);

        // No forwarded (float, float, float) constructor
        var ctor = typeof(StrictPosition).GetConstructor([typeof(float), typeof(float), typeof(float)]);
        Assert.Null(ctor);

        // Primary constructor still works
        var pos = new StrictPosition(new Vector3(1, 2, 3));
        Assert.Equal(new Vector3(1, 2, 3), pos.Value);
    }

    [Fact]
    public void MethodImplDefault_NoAggressiveInlining()
    {
        var addMethod = typeof(RelaxedId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "op_Addition");

        Assert.NotNull(addMethod);
        var flags = addMethod.MethodImplementationFlags;
        Assert.Equal(0, (int)(flags & MethodImplAttributes.AggressiveInlining));

        // Arithmetic still works
        var result = new RelaxedId(3) + new RelaxedId(4);
        Assert.Equal(7, result.Value);
    }

    // ── "Extending Your Types" example ──

    [Fact]
    public void CustomCrossTypeOperator_PositionPlusVelocity()
    {
        var pos = new Position(1, 2, 3);
        var vel = new Velocity(0.1f, 0, 0);
        Position updated = pos + vel;
        Assert.Equal(1.1f, updated.X, 0.001f);
        Assert.Equal(2f, updated.Y);
        Assert.Equal(3f, updated.Z);
    }
}
