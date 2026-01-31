using System.Numerics;
using Xunit;

public class StaticMemberTests
{
    [Fact]
    public void Zero_MatchesVector3Zero()
    {
        Assert.Equal(Vector3.Zero, (Vector3)Position.Zero);
    }

    [Fact]
    public void One_MatchesVector3One()
    {
        Assert.Equal(Vector3.One, (Vector3)Position.One);
    }

    [Fact]
    public void UnitX_MatchesVector3UnitX()
    {
        Assert.Equal(Vector3.UnitX, (Vector3)Position.UnitX);
    }

    [Fact]
    public void UnitY_MatchesVector3UnitY()
    {
        Assert.Equal(Vector3.UnitY, (Vector3)Position.UnitY);
    }

    [Fact]
    public void UnitZ_MatchesVector3UnitZ()
    {
        Assert.Equal(Vector3.UnitZ, (Vector3)Position.UnitZ);
    }

    [Fact]
    public void StaticMembers_ReturnAliasType()
    {
        // Verify the static members return the alias type, not the underlying type
        Position zero = Position.Zero;
        Position one = Position.One;
        Assert.Equal(new Vector3(0, 0, 0), zero.Value);
        Assert.Equal(new Vector3(1, 1, 1), one.Value);
    }
}
