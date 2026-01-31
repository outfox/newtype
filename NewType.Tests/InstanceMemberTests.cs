using System.Numerics;
using Xunit;

namespace newtype.tests;

public class InstanceMemberTests
{
    [Fact]
    public void X_ReturnsCorrectComponent()
    {
        Position p = new Vector3(1, 2, 3);
        Assert.Equal(1f, p.X);
    }

    [Fact]
    public void Y_ReturnsCorrectComponent()
    {
        Position p = new Vector3(1, 2, 3);
        Assert.Equal(2f, p.Y);
    }

    [Fact]
    public void Z_ReturnsCorrectComponent()
    {
        Position p = new Vector3(1, 2, 3);
        Assert.Equal(3f, p.Z);
    }

    [Fact]
    public void Length_MatchesUnderlying()
    {
        var vec = new Vector3(3, 4, 0);
        Position p = vec;
        Assert.Equal(vec.Length(), p.Length());
    }

    [Fact]
    public void LengthSquared_MatchesUnderlying()
    {
        var vec = new Vector3(3, 4, 0);
        Position p = vec;
        Assert.Equal(vec.LengthSquared(), p.LengthSquared());
    }

    [Fact]
    public void Length_KnownValue()
    {
        // 3-4-5 right triangle in XY plane
        Position p = new Vector3(3, 4, 0);
        Assert.Equal(5f, p.Length());
    }

    [Fact]
    public void LengthSquared_KnownValue()
    {
        Position p = new Vector3(3, 4, 0);
        Assert.Equal(25f, p.LengthSquared());
    }
}