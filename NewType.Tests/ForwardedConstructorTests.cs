using System.Numerics;
using Xunit;

namespace newtype.tests;

public class ForwardedConstructorTests
{
    // --- Vector3-based (Position) ---

    [Fact]
    public void Position_ScalarConstructor()
    {
        var p = new Position(5f);
        Assert.Equal(new Vector3(5f), p.Value);
    }

    [Fact]
    public void Position_XYZConstructor()
    {
        var p = new Position(1f, 2f, 3f);
        Assert.Equal(new Vector3(1f, 2f, 3f), p.Value);
    }

    [Fact]
    public void Position_Vector2PlusZConstructor()
    {
        var p = new Position(new Vector2(1f, 2f), 3f);
        Assert.Equal(new Vector3(1f, 2f, 3f), p.Value);
    }

    [Fact]
    public void Position_ReadOnlySpanConstructor()
    {
        ReadOnlySpan<float> values = [4f, 5f, 6f];
        var p = new Position(values);
        Assert.Equal(new Vector3(4f, 5f, 6f), p.Value);
    }

    // --- string-based (Name) ---

    [Fact]
    public void Name_CharCountConstructor()
    {
        var n = new Name('x', 3);
        Assert.Equal(new string('x', 3), n.Value);
    }

    [Fact]
    public void Name_CharArrayConstructor()
    {
        var n = new Name(new[] { 'a', 'b', 'c' });
        Assert.Equal("abc", n.Value);
    }

    [Fact]
    public void Name_CharArraySliceConstructor()
    {
        var n = new Name(new[] { 'a', 'b', 'c', 'd' }, 1, 2);
        Assert.Equal("bc", n.Value);
    }

    [Fact]
    public void Name_ReadOnlySpanConstructor()
    {
        ReadOnlySpan<char> chars = ['h', 'i'];
        var n = new Name(chars);
        Assert.Equal("hi", n.Value);
    }

    // --- Primitive (EntityId wrapping int) — no forwarded constructors ---

    [Fact]
    public void EntityId_OnlyHasPrimaryConstructor()
    {
        // Primitives have no discoverable constructors, so only the "from T" ctor exists.
        var id = new EntityId(42);
        Assert.Equal(42, id.Value);
    }

    // --- Class with constructor (ContactEmail wrapping EmailAddress) ---

    [Fact]
    public void ContactEmail_ForwardedConstructor()
    {
        var email = new ContactEmail("alice", "example.com");
        Assert.Equal("alice", email.Value.User);
        Assert.Equal("example.com", email.Value.Domain);
    }

    // --- Class with constructor (Price wrapping Money) ---

    [Fact]
    public void Price_ForwardedConstructor()
    {
        var price = new Price(9.99m, "USD");
        Assert.Equal(9.99m, price.Value.Amount);
        Assert.Equal("USD", price.Value.Currency);
    }

    // --- Record with primary constructor (Tint wrapping Rgb) ---

    [Fact]
    public void Tint_ForwardedRecordConstructor()
    {
        var tint = new Tint(255, 128, 0);
        Assert.Equal((byte)255, tint.Value.R);
        Assert.Equal((byte)128, tint.Value.G);
        Assert.Equal((byte)0, tint.Value.B);
    }
}
