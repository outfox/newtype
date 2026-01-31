using Xunit;

namespace newtype.tests;

/// <summary>
/// Tests for newtype wrappers using partial record (record class) declarations.
/// RecordEntityId wraps int, RecordTimestamp wraps double, RecordTint wraps Rgb.
/// </summary>
public class RecordClassTests
{
    // =========================================================================
    // RecordEntityId — record class wrapping int
    // =========================================================================

    // --- Construction and Implicit Conversions ---

    [Fact]
    public void RecordEntityId_Construction()
    {
        var id = new RecordEntityId(42);
        Assert.Equal(42, id.Value);
    }

    [Fact]
    public void RecordEntityId_ImplicitConversion_FromInt()
    {
        RecordEntityId id = 42;
        Assert.Equal(42, id.Value);
    }

    [Fact]
    public void RecordEntityId_ImplicitConversion_ToInt()
    {
        RecordEntityId id = 42;
        int raw = id;
        Assert.Equal(42, raw);
    }

    [Fact]
    public void RecordEntityId_RoundTrip()
    {
        RecordEntityId id = 99;
        int raw = id;
        RecordEntityId back = raw;
        Assert.Equal(id.Value, back.Value);
    }

    // --- Record-synthesized equality (compares underlying value) ---

    [Fact]
    public void RecordEntityId_Equality_RecordSynthesized()
    {
        RecordEntityId a = 42;
        RecordEntityId b = 42;
        RecordEntityId c = 99;

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void RecordEntityId_GetHashCode_ConsistentWithEquals()
    {
        RecordEntityId a = 42;
        RecordEntityId b = 42;
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // --- Arithmetic Operators ---

    [Fact]
    public void RecordEntityId_Addition()
    {
        RecordEntityId a = 10;
        RecordEntityId b = 20;
        RecordEntityId result = a + b;
        Assert.Equal(30, result.Value);
    }

    [Fact]
    public void RecordEntityId_Subtraction()
    {
        RecordEntityId a = 30;
        RecordEntityId b = 10;
        RecordEntityId result = a - b;
        Assert.Equal(20, result.Value);
    }

    [Fact]
    public void RecordEntityId_MixedArithmetic()
    {
        RecordEntityId id = 10;
        RecordEntityId result = id * 3;
        Assert.Equal(30, result.Value);

        RecordEntityId result2 = 3 * id;
        Assert.Equal(30, result2.Value);
    }

    // --- Comparison Operators ---

    [Fact]
    public void RecordEntityId_Comparison()
    {
        RecordEntityId low = 1;
        RecordEntityId high = 100;

        Assert.True(low < high);
        Assert.True(high > low);
        Assert.True(low <= high);
        Assert.True(high >= low);
    }

    // --- Unary Operators ---

    [Fact]
    public void RecordEntityId_UnaryNegation()
    {
        RecordEntityId id = 42;
        RecordEntityId neg = -id;
        Assert.Equal(-42, neg.Value);
    }

    // --- ToString prints underlying value, not record format ---

    [Fact]
    public void RecordEntityId_ToString_PrintsUnderlyingValue()
    {
        RecordEntityId id = 42;
        Assert.Equal("42", id.ToString());
    }

    // --- Default is null ---

    [Fact]
    public void RecordEntityId_Default_IsNull()
    {
        RecordEntityId id = default!;
        Assert.Null(id);
    }

    // =========================================================================
    // RecordTimestamp — record class wrapping double
    // =========================================================================

    [Fact]
    public void RecordTimestamp_ImplicitConversion_RoundTrip()
    {
        RecordTimestamp ts = 3.14;
        double raw = ts;
        Assert.Equal(3.14, raw);
    }

    [Fact]
    public void RecordTimestamp_Equality()
    {
        RecordTimestamp a = 1.0;
        RecordTimestamp b = 1.0;
        RecordTimestamp c = 2.0;

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void RecordTimestamp_Arithmetic()
    {
        RecordTimestamp a = 10.5;
        RecordTimestamp b = 4.5;
        RecordTimestamp result = a + b;
        Assert.Equal(15.0, result.Value);
    }

    [Fact]
    public void RecordTimestamp_MixedArithmetic()
    {
        RecordTimestamp ts = 5.0;
        RecordTimestamp result = ts * 2.0;
        Assert.Equal(10.0, result.Value);

        RecordTimestamp result2 = 2.0 * ts;
        Assert.Equal(10.0, result2.Value);
    }

    [Fact]
    public void RecordTimestamp_Comparison()
    {
        RecordTimestamp early = 1.0;
        RecordTimestamp late = 60.0;

        Assert.True(early < late);
        Assert.True(late > early);
    }

    [Fact]
    public void RecordTimestamp_ToString()
    {
        RecordTimestamp ts = 42.0;
        Assert.Equal(42.0.ToString(), ts.ToString());
    }

    // =========================================================================
    // RecordTint — record class wrapping Rgb (record reference type)
    // =========================================================================

    [Fact]
    public void RecordTint_ImplicitConversion_FromUnderlying()
    {
        var rgb = new Rgb(255, 128, 0);
        RecordTint tint = rgb;
        Assert.Equal(255, tint.R);
        Assert.Equal(128, tint.G);
        Assert.Equal(0, tint.B);
    }

    [Fact]
    public void RecordTint_ImplicitConversion_ToUnderlying()
    {
        RecordTint tint = new Rgb(0, 255, 0);
        Rgb raw = tint;
        Assert.Equal(new Rgb(0, 255, 0), raw);
    }

    [Fact]
    public void RecordTint_RoundTrip()
    {
        var original = new Rgb(100, 150, 200);
        RecordTint tint = original;
        Rgb result = tint;
        Assert.Equal(original, result);
    }

    [Fact]
    public void RecordTint_Equality()
    {
        RecordTint a = new Rgb(255, 0, 0);
        RecordTint b = new Rgb(255, 0, 0);
        RecordTint c = new Rgb(0, 255, 0);

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void RecordTint_GetHashCode_ConsistentWithEquals()
    {
        RecordTint a = new Rgb(128, 128, 128);
        RecordTint b = new Rgb(128, 128, 128);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void RecordTint_PropertyForwarding()
    {
        RecordTint tint = new Rgb(10, 20, 30);
        Assert.Equal(10, tint.R);
        Assert.Equal(20, tint.G);
        Assert.Equal(30, tint.B);
    }

    [Fact]
    public void RecordTint_MethodForwarding_Invert()
    {
        RecordTint tint = new Rgb(255, 0, 128);
        RecordTint inverted = tint.Invert();
        Assert.Equal(0, inverted.R);
        Assert.Equal(255, inverted.G);
        Assert.Equal(127, inverted.B);
    }

    [Fact]
    public void RecordTint_MethodForwarding_Mix()
    {
        RecordTint a = new Rgb(200, 100, 0);
        RecordTint b = new Rgb(100, 200, 100);
        RecordTint mixed = a.Mix(b);
        Assert.Equal(150, mixed.R);
        Assert.Equal(150, mixed.G);
        Assert.Equal(50, mixed.B);
    }

    [Fact]
    public void RecordTint_ToString()
    {
        RecordTint tint = new Rgb(255, 128, 0);
        Assert.Equal("#FF8000", tint.ToString());
    }
}
