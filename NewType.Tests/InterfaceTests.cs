using System;
using System.Numerics;
using Xunit;

public class InterfaceTests
{
    [Fact]
    public void IEquatable_IsImplemented()
    {
        Position p = new Vector3(1, 2, 3);
        Assert.IsAssignableFrom<IEquatable<Position>>(p);
    }

    [Fact]
    public void IEquatable_Equals_ReturnsTrue_ForEqual()
    {
        IEquatable<Position> a = (Position)new Vector3(1, 2, 3);
        Position b = new Vector3(1, 2, 3);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void IEquatable_Equals_ReturnsFalse_ForDifferent()
    {
        IEquatable<Position> a = (Position)new Vector3(1, 2, 3);
        Position b = new Vector3(4, 5, 6);
        Assert.False(a.Equals(b));
    }

    [Fact]
    public void IFormattable_IsImplemented()
    {
        Position p = new Vector3(1, 2, 3);
        Assert.IsAssignableFrom<IFormattable>(p);
    }

    [Fact]
    public void IFormattable_ToString_WithFormat()
    {
        Position p = new Vector3(1.5f, 2.5f, 3.5f);
        var formatted = ((IFormattable)p).ToString("F1", null);
        Assert.NotNull(formatted);
        Assert.Contains("1.5", formatted);
    }

    [Fact]
    public void ToString_MatchesUnderlying()
    {
        var vec = new Vector3(1, 2, 3);
        Position p = vec;
        Assert.Equal(vec.ToString(), p.ToString());
    }
}
