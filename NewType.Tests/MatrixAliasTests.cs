using System.Numerics;
using Xunit;

public class MatrixAliasTests
{
    [Fact]
    public void Construction_FromIdentity()
    {
        Transform t = Matrix4x4.Identity;
        Assert.Equal(Matrix4x4.Identity, t.Value);
    }

    [Fact]
    public void IsIdentity_Forwarded()
    {
        Transform t = Matrix4x4.Identity;
        Assert.True(t.IsIdentity);
    }

    [Fact]
    public void IsIdentity_FalseForNonIdentity()
    {
        Transform t = new Matrix4x4(
            2, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);
        Assert.False(t.IsIdentity);
    }

    [Fact]
    public void M11_ElementAccess()
    {
        Transform t = Matrix4x4.Identity;
        Assert.Equal(1f, t.M11);
    }

    [Fact]
    public void Translation_ElementAccess()
    {
        var m = Matrix4x4.CreateTranslation(10, 20, 30);
        Transform t = m;
        Assert.Equal(10f, t.M41);
        Assert.Equal(20f, t.M42);
        Assert.Equal(30f, t.M43);
    }

    [Fact]
    public void ImplicitConversion_RoundTrip()
    {
        var m = Matrix4x4.CreateScale(2f);
        Transform t = m;
        Matrix4x4 result = t;
        Assert.Equal(m, result);
    }

    [Fact]
    public void Multiplication_TwoTransforms()
    {
        Transform a = Matrix4x4.Identity;
        Transform b = Matrix4x4.Identity;
        Transform result = a * b;
        Assert.Equal(Matrix4x4.Identity, result.Value);
    }

    [Fact]
    public void Negation()
    {
        Transform t = Matrix4x4.Identity;
        Transform neg = -t;
        Assert.Equal(-1f, neg.M11);
    }
}
