using System.Numerics;
using Xunit;

namespace newtype.tests;

public class QuaternionAliasTests
{
    [Fact]
    public void Construction_FromIdentity()
    {
        Rotation r = Quaternion.Identity;
        Assert.Equal(Quaternion.Identity, r.Value);
    }

    [Fact]
    public void IsIdentity_Forwarded()
    {
        Rotation r = Quaternion.Identity;
        Assert.True(r.IsIdentity);
    }

    [Fact]
    public void IsIdentity_FalseForNonIdentity()
    {
        Rotation r = new Quaternion(1, 0, 0, 1);
        Assert.False(r.IsIdentity);
    }

    [Fact]
    public void ImplicitConversion_RoundTrip()
    {
        var q = new Quaternion(0.5f, 0.5f, 0.5f, 0.5f);
        Rotation r = q;
        Quaternion result = r;
        Assert.Equal(q, result);
    }

    [Fact]
    public void Negation()
    {
        Rotation r = new Quaternion(1, 2, 3, 4);
        Rotation neg = -r;
        Assert.Equal(new Quaternion(-1, -2, -3, -4), neg.Value);
    }

    [Fact]
    public void Multiplication_TwoRotations()
    {
        Rotation a = Quaternion.Identity;
        Rotation b = Quaternion.Identity;
        Rotation result = a * b;
        Assert.Equal(Quaternion.Identity, result.Value);
    }
}