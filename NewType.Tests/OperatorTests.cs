using System.Numerics;
using Xunit;

public class OperatorTests
{
    [Fact]
    public void Addition_AliasAndAlias()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(4, 5, 6);
        Position result = a + b;
        Assert.Equal(new Vector3(5, 7, 9), result.Value);
    }

    [Fact]
    public void Addition_AliasAndUnderlying()
    {
        Position a = new Vector3(1, 2, 3);
        Position result = a + new Vector3(10, 20, 30);
        Assert.Equal(new Vector3(11, 22, 33), result.Value);
    }

    [Fact]
    public void Subtraction_AliasAndAlias()
    {
        Position a = new Vector3(5, 7, 9);
        Position b = new Vector3(1, 2, 3);
        Position result = a - b;
        Assert.Equal(new Vector3(4, 5, 6), result.Value);
    }

    [Fact]
    public void Multiplication_AliasAndScalar()
    {
        Position p = new Vector3(1, 2, 3);
        Position result = p * 2f;
        Assert.Equal(new Vector3(2, 4, 6), result.Value);
    }

    [Fact]
    public void Multiplication_ScalarAndAlias()
    {
        Position p = new Vector3(1, 2, 3);
        Position result = 3f * p;
        Assert.Equal(new Vector3(3, 6, 9), result.Value);
    }

    [Fact]
    public void Division_AliasAndScalar()
    {
        Position p = new Vector3(4, 6, 8);
        Position result = p / 2f;
        Assert.Equal(new Vector3(2, 3, 4), result.Value);
    }

    [Fact]
    public void Negation()
    {
        Position p = new Vector3(1, -2, 3);
        Position result = -p;
        Assert.Equal(new Vector3(-1, 2, -3), result.Value);
    }

    [Fact]
    public void MixedTypeArithmetic_ViaImplicitConversion()
    {
        Position p = new Vector3(1, 0, 0);
        Velocity v = new Vector3(0, 1, 0);
        // Position + Vector3 works; Velocity implicitly converts to Vector3
        Position result = p + (Vector3)v;
        Assert.Equal(new Vector3(1, 1, 0), result.Value);
    }
}
