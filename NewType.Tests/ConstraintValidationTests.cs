using System.Numerics;
using newtype.tests;
using Xunit;

public class ConstraintValidationTests
{
    [Fact]
    public void Direction_Valid()
    {
        //doesn't throw
        var dir = new Direction(new Vector2(0, 1));
    }

    [Fact]
    public void Direction_ValidImplicit()
    {
        //doesn't throw
        Direction dir = new Vector2(0, 1);
    }

    [Fact]
    public void Direction_Valid_Forwarded()
    {
        //doesn't throw
        var dir = new Direction(0, 1);
    }
    
    [Fact]
    public void Direction_CreateInvalid_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new Direction(new Vector2(999, 999)));
    }

    [Fact]
    public void Direction_CreateInvalidImplicit_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => (Direction)new Vector2(999, 999));
    }

    [Fact]
    public void Direction_CreateInvalidForwarded_Throws()
    {
        Assert.Throws<InvalidOperationException>(() => new Direction(999, 999));
    }
    
    [Fact]
    public void Direction_ForwardedOperation_ValidResult()
    {
        var dir1 = new Direction(new Vector2(0, 1f));
        var dir2 = new Direction(new Vector2(0, 1f));
        var dir3 = dir1 * dir2;
    }

    [Fact]
    public void Direction_ForwardedOperation_InvalidResult_Throws()
    {
        var dir1 = new Direction(new Vector2(0, 1f));
        var dir2 = new Direction(new Vector2(0, 1f));

        Assert.Throws<InvalidOperationException>(() => dir1 + dir2);
    }
}