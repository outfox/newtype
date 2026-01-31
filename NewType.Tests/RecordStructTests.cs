using Xunit;

namespace newtype.tests;

public class RecordStructTests
{
    // --- Score (record struct wrapping int) ---

    [Fact]
    public void Score_ImplicitConversion_RoundTrip()
    {
        Score s = 100;
        int raw = s;
        Assert.Equal(100, raw);
    }

    [Fact]
    public void Score_Equality()
    {
        Score a = 10;
        Score b = 10;
        Score c = 20;

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void Score_GetHashCode_MatchesUnderlying()
    {
        Score s = 42;
        Assert.Equal(42.GetHashCode(), s.GetHashCode());
    }

    [Fact]
    public void Score_Arithmetic()
    {
        Score a = 50;
        Score b = 30;

        Assert.Equal(80, (int) (a + b));
        Assert.Equal(20, (int) (a - b));
        Assert.Equal(1500, (int) (a * b));
    }

    [Fact]
    public void Score_MixedArithmetic()
    {
        Score s = 10;

        Assert.Equal(20, (int) (s * 2));
        Assert.Equal(20, (int) (2 * s));
        Assert.Equal(5, (int) (s / 2));
    }

    [Fact]
    public void Score_Comparison()
    {
        Score low = 1;
        Score high = 100;

        Assert.True(low < high);
        Assert.True(high > low);
        Assert.True(low <= high);
        Assert.True(high >= low);
    }

    [Fact]
    public void Score_UnaryOperators()
    {
        Score s = 5;

        Assert.Equal(-5, (int) (-s));
        Assert.Equal(5, (int) (+s));
        Assert.Equal(~5, (int) (~s));
    }

    [Fact]
    public void Score_Default()
    {
        Score s = default;
        Assert.Equal(0, (int) s);
    }

    [Fact]
    public void Score_ToString()
    {
        Score s = 99;
        Assert.Equal("99", s.ToString());
    }

    // --- Duration (readonly record struct wrapping double) ---

    [Fact]
    public void Duration_ImplicitConversion_RoundTrip()
    {
        Duration d = 3.5;
        double raw = d;
        Assert.Equal(3.5, raw);
    }

    [Fact]
    public void Duration_Equality()
    {
        Duration a = 1.0;
        Duration b = 1.0;
        Duration c = 2.0;

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void Duration_GetHashCode_MatchesUnderlying()
    {
        Duration d = 7.77;
        Assert.Equal(7.77.GetHashCode(), d.GetHashCode());
    }

    [Fact]
    public void Duration_Arithmetic()
    {
        Duration a = 10.5;
        Duration b = 4.5;

        Assert.Equal(15.0, (double) (a + b));
        Assert.Equal(6.0, (double) (a - b));
    }

    [Fact]
    public void Duration_MixedArithmetic()
    {
        Duration d = 5.0;

        Assert.Equal(10.0, (double) (d * 2.0));
        Assert.Equal(10.0, (double) (2.0 * d));
        Assert.Equal(2.5, (double) (d / 2.0));
    }

    [Fact]
    public void Duration_Comparison()
    {
        Duration short_ = 1.0;
        Duration long_ = 60.0;

        Assert.True(short_ < long_);
        Assert.True(long_ > short_);
        Assert.True(short_ <= long_);
        Assert.True(long_ >= short_);
    }

    [Fact]
    public void Duration_Negation()
    {
        Duration d = 3.0;
        Assert.Equal(-3.0, (double) (-d));
    }

    [Fact]
    public void Duration_Default()
    {
        Duration d = default;
        Assert.Equal(0.0, (double) d);
    }

    [Fact]
    public void Duration_ToString()
    {
        Duration d = 42.0;
        Assert.Equal(42.0.ToString(), d.ToString());
    }
}