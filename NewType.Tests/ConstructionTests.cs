using System.Numerics;
using Xunit;

public class ConstructionTests
{
    [Fact]
    public void ImplicitConversion_FromUnderlyingType()
    {
        Position p = new Vector3(1, 2, 3);
        Assert.Equal(new Vector3(1, 2, 3), p.Value);
    }

    [Fact]
    public void ImplicitConversion_ToUnderlyingType()
    {
        Position p = new Vector3(4, 5, 6);
        Vector3 v = p;
        Assert.Equal(new Vector3(4, 5, 6), v);
    }

    [Fact]
    public void RoundTrip_PreservesValue()
    {
        var original = new Vector3(7, 8, 9);
        Position p = original;
        Vector3 result = p;
        Assert.Equal(original, result);
    }

    [Fact]
    public void Constructor_SetsValue()
    {
        var vec = new Vector3(10, 20, 30);
        var p = new Position(vec);
        Assert.Equal(vec, p.Value);
    }

    [Fact]
    public void Default_IsZeroVector()
    {
        var p = default(Position);
        Assert.Equal(Vector3.Zero, p.Value);
    }
}
