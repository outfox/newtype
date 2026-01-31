using System.Numerics;
using Xunit;

namespace newtype.tests;

public class EqualityTests
{
    [Fact]
    public void EqualityOperator_EqualValues()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(1, 2, 3);
        Assert.True(a == b);
    }

    [Fact]
    public void EqualityOperator_DifferentValues()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(4, 5, 6);
        Assert.False(a == b);
    }

    [Fact]
    public void InequalityOperator_DifferentValues()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(4, 5, 6);
        Assert.True(a != b);
    }

    [Fact]
    public void InequalityOperator_EqualValues()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(1, 2, 3);
        Assert.False(a != b);
    }

    [Fact]
    public void Equals_TypedOverload()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(1, 2, 3);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_ObjectOverload_SameType()
    {
        Position a = new Vector3(1, 2, 3);
        object b = (Position) new Vector3(1, 2, 3);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Equals_ObjectOverload_DifferentType()
    {
        Position a = new Vector3(1, 2, 3);
        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.False(a.Equals("not a position"));
    }

    [Fact]
    public void GetHashCode_ConsistentForEqualValues()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(1, 2, 3);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void GetHashCode_MatchesUnderlying()
    {
        var vec = new Vector3(1, 2, 3);
        Position p = vec;
        Assert.Equal(vec.GetHashCode(), p.GetHashCode());
    }
}