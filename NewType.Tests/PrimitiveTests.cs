using System.Globalization;
using Xunit;

namespace newtype.tests;

public class PrimitiveTests
{
    // --- EntityId (int) ---

    [Fact]
    public void EntityId_ImplicitConversion_RoundTrip()
    {
        EntityId id = 42;
        int raw = id;
        Assert.Equal(42, raw);
    }

    [Fact]
    public void EntityId_Equality()
    {
        EntityId a = 1;
        EntityId b = 1;
        EntityId c = 2;

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void EntityId_GetHashCode_MatchesUnderlying()
    {
        EntityId id = 123;
        Assert.Equal(123.GetHashCode(), id.GetHashCode());
    }

    [Fact]
    public void EntityId_Arithmetic()
    {
        EntityId a = 10;
        EntityId b = 3;

        Assert.Equal(13, (int) (a + b));
        Assert.Equal(7, (int) (a - b));
        Assert.Equal(30, (int) (a * b));
        Assert.Equal(3, (int) (a / b));
    }

    [Fact]
    public void EntityId_Comparison()
    {
        EntityId low = 1;
        EntityId high = 100;

        Assert.True(low < high);
        Assert.True(high > low);
        Assert.True(low <= high);
        Assert.True(high >= low);
        Assert.True(low <= 1);
        Assert.True(low >= 1);
    }

    [Fact]
    public void EntityId_UnaryOperators()
    {
        EntityId id = 5;

        Assert.Equal(-5, (int) (-id));
        Assert.Equal(5, (int) (+id));
        Assert.Equal(~5, (int) (~id));
    }

    [Fact]
    public void EntityId_BitwiseOperators()
    {
        EntityId a = 0b1100;
        EntityId b = 0b1010;

        Assert.Equal(0b1000, (int) (a & b));
        Assert.Equal(0b1110, (int) (a | b));
        Assert.Equal(0b0110, (int) (a ^ b));
    }

    [Fact]
    public void EntityId_ShiftOperators()
    {
        EntityId a = 1;

        Assert.Equal(8, (int) (a << 3));
        EntityId big = 16;
        Assert.Equal(2, (int) (big >> 3));
    }

    [Fact]
    public void EntityId_Default()
    {
        EntityId id = default;
        Assert.Equal(0, (int) id);
    }

    [Fact]
    public void EntityId_ToString()
    {
        EntityId id = 42;
        Assert.Equal("42", id.ToString());
    }

    // --- Health (float) ---

    [Fact]
    public void Health_ImplicitConversion_RoundTrip()
    {
        Health h = 100.0f;
        float raw = h;
        Assert.Equal(100.0f, raw);
    }

    [Fact]
    public void Health_Arithmetic()
    {
        Health a = 80.0f;
        Health damage = 25.0f;

        var remaining = a - damage;
        Assert.Equal(55.0f, (float) remaining);

        var healed = remaining + (Health) 10.0f;
        Assert.Equal(65.0f, (float) healed);
    }

    [Fact]
    public void Health_MixedArithmetic()
    {
        Health h = 100.0f;

        // alias op T
        var halved = h * 0.5f;
        Assert.Equal(50.0f, (float) halved);

        // T op alias
        var doubled = 2.0f * h;
        Assert.Equal(200.0f, (float) doubled);
    }

    [Fact]
    public void Health_Comparison()
    {
        Health low = 10.0f;
        Health high = 90.0f;

        Assert.True(low < high);
        Assert.True(high > low);
    }

    [Fact]
    public void Health_Equality()
    {
        Health a = 50.0f;
        Health b = 50.0f;

        Assert.True(a == b);
        Assert.Equal(a, b);
    }

    [Fact]
    public void Health_Negation()
    {
        Health h = 10.0f;
        Assert.Equal(-10.0f, (float) (-h));
    }

    [Fact]
    public void Health_ToString()
    {
        Health h = 3.14f;
        Assert.Equal(3.14f.ToString(CultureInfo.InvariantCulture), h.ToString());
    }

    // --- Timestamp (double) ---

    [Fact]
    public void Timestamp_ImplicitConversion_RoundTrip()
    {
        Timestamp t = 1706745600.123;
        double raw = t;
        Assert.Equal(1706745600.123, raw);
    }

    [Fact]
    public void Timestamp_Arithmetic()
    {
        Timestamp start = 100.0;
        Timestamp duration = 42.5;

        var end = start + duration;
        Assert.Equal(142.5, (double) end);

        var diff = end - start;
        Assert.Equal(42.5, (double) diff);
    }

    [Fact]
    public void Timestamp_Comparison()
    {
        Timestamp earlier = 1000.0;
        Timestamp later = 2000.0;

        Assert.True(earlier < later);
        Assert.True(later > earlier);
        Assert.False(earlier == later);
    }

    [Fact]
    public void Timestamp_Equality()
    {
        Timestamp a = 999.999;
        Timestamp b = 999.999;
        Assert.True(a == b);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Timestamp_Default()
    {
        Timestamp t = default;
        Assert.Equal(0.0, (double) t);
    }

    // --- IsActive (bool) ---

    [Fact]
    public void IsActive_ImplicitConversion_RoundTrip()
    {
        IsActive active = true;
        bool raw = active;
        Assert.True(raw);

        IsActive inactive = false;
        Assert.False(inactive);
    }

    [Fact]
    public void IsActive_Equality()
    {
        IsActive a = true;
        IsActive b = true;
        IsActive c = false;

        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void IsActive_LogicalOperators()
    {
        IsActive a = true;
        IsActive b = false;

        // bool has & and | (non-short-circuit)
        Assert.False(a & b);
        Assert.True(a | b);
        Assert.True(a ^ b);
    }

    [Fact]
    public void IsActive_Negation()
    {
        IsActive active = true;
        Assert.False(!active);

        IsActive inactive = false;
        Assert.True(!inactive);
    }

    [Fact]
    public void IsActive_Default()
    {
        IsActive flag = default;
        Assert.False(flag);
    }

    [Fact]
    public void IsActive_ToString()
    {
        IsActive a = true;
        IsActive b = false;
        Assert.Equal("True", a.ToString());
        Assert.Equal("False", b.ToString());
    }

    // --- Ref / ECS patterns with primitives ---

    [Fact]
    public void EntityId_RefLocal_ArrayAccess()
    {
        var ids = new EntityId[] {1, 2, 3};

        ref var id = ref ids[1];
        id = 42;

        Assert.Equal(42, (int) ids[1]);
    }

    [Fact]
    public void Health_RefIteration_DamageAll()
    {
        var healths = new Health[] {100.0f, 80.0f, 60.0f};
        Health damage = 25.0f;

        for (var i = 0; i < healths.Length; i++)
        {
            ref var h = ref healths[i];
            h -= damage;
        }

        Assert.Equal(75.0f, (float) healths[0]);
        Assert.Equal(55.0f, (float) healths[1]);
        Assert.Equal(35.0f, (float) healths[2]);
    }

    [Fact]
    public void IsActive_RefToggle()
    {
        var flags = new IsActive[] {true, false, true};

        for (var i = 0; i < flags.Length; i++)
        {
            ref var f = ref flags[i];
            f = !f;
        }

        Assert.False(flags[0]);
        Assert.True(flags[1]);
        Assert.False(flags[2]);
    }
}