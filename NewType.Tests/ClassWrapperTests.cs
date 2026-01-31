using Xunit;

namespace newtype.tests;

/// <summary>
/// Tests for newtype wrappers using partial class declarations.
/// ClassEntityId wraps int, ClassName wraps string, ClassContactEmail wraps EmailAddress.
/// </summary>
public class ClassWrapperTests
{
    // =========================================================================
    // ClassEntityId — wraps int
    // =========================================================================

    // --- Construction and Implicit Conversions ---

    [Fact]
    public void ClassEntityId_Construction()
    {
        var id = new ClassEntityId(42);
        Assert.Equal(42, id.Value);
    }

    [Fact]
    public void ClassEntityId_ImplicitConversion_FromInt()
    {
        ClassEntityId id = 42;
        Assert.Equal(42, id.Value);
    }

    [Fact]
    public void ClassEntityId_ImplicitConversion_ToInt()
    {
        ClassEntityId id = 42;
        int raw = id;
        Assert.Equal(42, raw);
    }

    [Fact]
    public void ClassEntityId_RoundTrip()
    {
        ClassEntityId id = 99;
        int raw = id;
        ClassEntityId back = raw;
        Assert.Equal(id.Value, back.Value);
    }

    // --- Arithmetic Operators ---

    [Fact]
    public void ClassEntityId_Addition()
    {
        ClassEntityId a = 10;
        ClassEntityId b = 20;
        ClassEntityId result = a + b;
        Assert.Equal(30, result.Value);
    }

    [Fact]
    public void ClassEntityId_Subtraction()
    {
        ClassEntityId a = 30;
        ClassEntityId b = 10;
        ClassEntityId result = a - b;
        Assert.Equal(20, result.Value);
    }

    [Fact]
    public void ClassEntityId_Multiplication()
    {
        ClassEntityId a = 5;
        ClassEntityId b = 6;
        ClassEntityId result = a * b;
        Assert.Equal(30, result.Value);
    }

    [Fact]
    public void ClassEntityId_MixedArithmetic()
    {
        ClassEntityId id = 10;
        ClassEntityId result = id + 5;
        Assert.Equal(15, result.Value);

        ClassEntityId result2 = 5 + id;
        Assert.Equal(15, result2.Value);
    }

    // --- Comparison Operators ---

    [Fact]
    public void ClassEntityId_Comparison()
    {
        ClassEntityId low = 1;
        ClassEntityId high = 100;

        Assert.True(low < high);
        Assert.True(high > low);
        Assert.True(low <= high);
        Assert.True(high >= low);
    }

    // --- Unary Operators ---

    [Fact]
    public void ClassEntityId_UnaryNegation()
    {
        ClassEntityId id = 42;
        ClassEntityId neg = -id;
        Assert.Equal(-42, neg.Value);
    }

    [Fact]
    public void ClassEntityId_UnaryPlus()
    {
        ClassEntityId id = 42;
        ClassEntityId pos = +id;
        Assert.Equal(42, pos.Value);
    }

    // --- Equality ---

    [Fact]
    public void ClassEntityId_Equality()
    {
        ClassEntityId a = 42;
        ClassEntityId b = 42;
        ClassEntityId c = 99;

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    // --- GetHashCode ---

    [Fact]
    public void ClassEntityId_GetHashCode_MatchesUnderlying()
    {
        ClassEntityId id = 42;
        Assert.Equal(42.GetHashCode(), id.GetHashCode());
    }

    [Fact]
    public void ClassEntityId_GetHashCode_ConsistentWithEquals()
    {
        ClassEntityId a = 42;
        ClassEntityId b = 42;
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // --- ToString ---

    [Fact]
    public void ClassEntityId_ToString()
    {
        ClassEntityId id = 42;
        Assert.Equal("42", id.ToString());
    }

    // --- Default is null ---

    [Fact]
    public void ClassEntityId_Default_IsNull()
    {
        ClassEntityId id = default!;
        Assert.Null(id);
    }

    // =========================================================================
    // ClassName — wraps string
    // =========================================================================

    [Fact]
    public void ClassName_ImplicitConversion_FromString()
    {
        ClassName name = "Alice";
        Assert.Equal("Alice", name.Value);
    }

    [Fact]
    public void ClassName_ImplicitConversion_ToString()
    {
        ClassName name = "Bob";
        string raw = name;
        Assert.Equal("Bob", raw);
    }

    [Fact]
    public void ClassName_RoundTrip()
    {
        ClassName name = "Charlie";
        string raw = name;
        ClassName back = raw;
        Assert.Equal("Charlie", back.Value);
    }

    [Fact]
    public void ClassName_Concatenation()
    {
        ClassName a = "Hello";
        ClassName b = " World";
        ClassName result = a + b;
        Assert.Equal("Hello World", result.Value);
    }

    [Fact]
    public void ClassName_Equality()
    {
        ClassName a = "Test";
        ClassName b = "Test";
        ClassName c = "Other";

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void ClassName_GetHashCode_ConsistentWithEquals()
    {
        ClassName a = "Test";
        ClassName b = "Test";
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ClassName_ToString()
    {
        ClassName name = "Alice";
        Assert.Equal("Alice", name.ToString());
    }

    // =========================================================================
    // ClassContactEmail — wraps EmailAddress (reference type)
    // =========================================================================

    [Fact]
    public void ClassContactEmail_Construction()
    {
        var email = new EmailAddress("alice", "example.com");
        ClassContactEmail contact = email;
        Assert.Equal("alice", contact.User);
        Assert.Equal("example.com", contact.Domain);
    }

    [Fact]
    public void ClassContactEmail_ImplicitConversion_ToUnderlying()
    {
        ClassContactEmail contact = new EmailAddress("bob", "test.com");
        EmailAddress raw = contact;
        Assert.Equal("bob", raw.User);
    }

    [Fact]
    public void ClassContactEmail_RoundTrip()
    {
        var original = new EmailAddress("charlie", "domain.org");
        ClassContactEmail contact = original;
        EmailAddress result = contact;
        Assert.Equal(original, result);
    }

    [Fact]
    public void ClassContactEmail_ConstructorForwarding()
    {
        var contact = new ClassContactEmail("alice", "example.com");
        Assert.Equal("alice", contact.User);
        Assert.Equal("example.com", contact.Domain);
    }

    [Fact]
    public void ClassContactEmail_PropertyForwarding()
    {
        ClassContactEmail contact = new EmailAddress("alice", "example.com");
        Assert.Equal("alice", contact.User);
        Assert.Equal("example.com", contact.Domain);
    }

    [Fact]
    public void ClassContactEmail_MethodForwarding()
    {
        ClassContactEmail contact = new EmailAddress("alice", "example.com");
        ClassContactEmail updated = contact.WithDomain("newdomain.org");
        Assert.Equal("alice", updated.User);
        Assert.Equal("newdomain.org", updated.Domain);
    }

    [Fact]
    public void ClassContactEmail_Equality()
    {
        ClassContactEmail a = new EmailAddress("alice", "example.com");
        ClassContactEmail b = new EmailAddress("alice", "example.com");
        ClassContactEmail c = new EmailAddress("bob", "example.com");

        Assert.Equal(a, b);
        Assert.NotEqual(a, c);
        Assert.True(a == b);
        Assert.True(a != c);
    }

    [Fact]
    public void ClassContactEmail_GetHashCode_MatchesUnderlying()
    {
        var email = new EmailAddress("alice", "example.com");
        ClassContactEmail contact = email;
        Assert.Equal(email.GetHashCode(), contact.GetHashCode());
    }

    [Fact]
    public void ClassContactEmail_Comparison()
    {
        ClassContactEmail a = new EmailAddress("alice", "example.com");
        ClassContactEmail b = new EmailAddress("bob", "example.com");

        Assert.True(a < b);
        Assert.True(b > a);
        Assert.True(a <= b);
        Assert.True(b >= a);
    }

    [Fact]
    public void ClassContactEmail_CompareTo()
    {
        ClassContactEmail a = new EmailAddress("alice", "example.com");
        ClassContactEmail b = new EmailAddress("bob", "example.com");

        Assert.True(a.CompareTo(b) < 0);
        Assert.True(b.CompareTo(a) > 0);
    }

    [Fact]
    public void ClassContactEmail_ToString()
    {
        ClassContactEmail contact = new EmailAddress("alice", "example.com");
        Assert.Equal("alice@example.com", contact.ToString());
    }
}
