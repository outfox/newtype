using Xunit;

namespace newtype.tests;

/// <summary>
/// Tests for newtype wrappers around custom reference types:
/// ContactEmail (class with IComparable), Price (class with operators), Tint (record).
/// </summary>
public class ReferenceTypeTests
{
    // =========================================================================
    // ContactEmail — wraps EmailAddress (class with IEquatable + IComparable)
    // =========================================================================

    // --- Implicit Conversions ---

    [Fact]
    public void ContactEmail_ImplicitConversion_FromUnderlying()
    {
        var email = new EmailAddress("alice", "example.com");
        ContactEmail contact = email;
        Assert.Equal("alice", contact.User);
        Assert.Equal("example.com", contact.Domain);
    }

    [Fact]
    public void ContactEmail_ImplicitConversion_ToUnderlying()
    {
        ContactEmail contact = new EmailAddress("bob", "test.com");
        EmailAddress raw = contact;
        Assert.Equal("bob", raw.User);
        Assert.Equal("test.com", raw.Domain);
    }

    [Fact]
    public void ContactEmail_RoundTrip()
    {
        var original = new EmailAddress("charlie", "domain.org");
        ContactEmail contact = original;
        EmailAddress result = contact;
        Assert.Equal(original, result);
    }

    // --- Equality ---

    [Fact]
    public void ContactEmail_Equality_Equal()
    {
        ContactEmail a = new EmailAddress("alice", "example.com");
        ContactEmail b = new EmailAddress("alice", "example.com");

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void ContactEmail_Equality_NotEqual()
    {
        ContactEmail a = new EmailAddress("alice", "example.com");
        ContactEmail b = new EmailAddress("bob", "example.com");

        Assert.NotEqual(a, b);
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void ContactEmail_GetHashCode_MatchesUnderlying()
    {
        var email = new EmailAddress("alice", "example.com");
        ContactEmail contact = email;
        Assert.Equal(email.GetHashCode(), contact.GetHashCode());
    }

    [Fact]
    public void ContactEmail_GetHashCode_ConsistentWithEquals()
    {
        ContactEmail a = new EmailAddress("alice", "example.com");
        ContactEmail b = new EmailAddress("alice", "example.com");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // --- Comparison Operators ---

    [Fact]
    public void ContactEmail_Comparison_LessThan()
    {
        ContactEmail a = new EmailAddress("alice", "example.com");
        ContactEmail b = new EmailAddress("bob", "example.com");

        // "alice@example.com" < "bob@example.com" lexicographically
        Assert.True(a < b);
        Assert.False(b < a);
    }

    [Fact]
    public void ContactEmail_Comparison_GreaterThan()
    {
        ContactEmail a = new EmailAddress("bob", "example.com");
        ContactEmail b = new EmailAddress("alice", "example.com");

        Assert.True(a > b);
        Assert.False(b > a);
    }

    [Fact]
    public void ContactEmail_Comparison_LessThanOrEqual()
    {
        ContactEmail a = new EmailAddress("alice", "example.com");
        ContactEmail b = new EmailAddress("bob", "example.com");
        ContactEmail c = new EmailAddress("alice", "example.com");

        Assert.True(a <= b);
        Assert.True(a <= c);
        Assert.False(b <= a);
    }

    [Fact]
    public void ContactEmail_Comparison_GreaterThanOrEqual()
    {
        ContactEmail a = new EmailAddress("bob", "example.com");
        ContactEmail b = new EmailAddress("alice", "example.com");
        ContactEmail c = new EmailAddress("bob", "example.com");

        Assert.True(a >= b);
        Assert.True(a >= c);
        Assert.False(b >= a);
    }

    // --- CompareTo ---

    [Fact]
    public void ContactEmail_CompareTo()
    {
        ContactEmail a = new EmailAddress("alice", "example.com");
        ContactEmail b = new EmailAddress("bob", "example.com");
        ContactEmail c = new EmailAddress("alice", "example.com");

        Assert.True(a.CompareTo(b) < 0);
        Assert.True(b.CompareTo(a) > 0);
        Assert.Equal(0, a.CompareTo(c));
    }

    // --- Property Forwarding ---

    [Fact]
    public void ContactEmail_PropertyForwarding_User()
    {
        ContactEmail contact = new EmailAddress("alice", "example.com");
        Assert.Equal("alice", contact.User);
    }

    [Fact]
    public void ContactEmail_PropertyForwarding_Domain()
    {
        ContactEmail contact = new EmailAddress("alice", "example.com");
        Assert.Equal("example.com", contact.Domain);
    }

    // --- Method Forwarding with Return Type Wrapping ---

    [Fact]
    public void ContactEmail_WithDomain_ReturnsContactEmail()
    {
        ContactEmail contact = new EmailAddress("alice", "example.com");
        ContactEmail updated = contact.WithDomain("newdomain.org");

        Assert.IsType<ContactEmail>(updated);
        Assert.Equal("alice", updated.User);
        Assert.Equal("newdomain.org", updated.Domain);
    }

    // --- ToString ---

    [Fact]
    public void ContactEmail_ToString()
    {
        ContactEmail contact = new EmailAddress("alice", "example.com");
        Assert.Equal("alice@example.com", contact.ToString());
    }

    // --- Default (null underlying) ---

    [Fact]
    public void ContactEmail_Default_HasNullUnderlying()
    {
        ContactEmail contact = default;
        Assert.Null(contact.Value);
    }

    [Fact]
    public void ContactEmail_Default_ToStringReturnsEmpty()
    {
        ContactEmail contact = default;
        Assert.Equal("", contact.ToString());
    }

    [Fact]
    public void ContactEmail_Default_GetHashCodeDoesNotThrow()
    {
        ContactEmail contact = default;
        Assert.Equal(0, contact.GetHashCode());
    }

    [Fact]
    public void ContactEmail_Default_EqualsDefault()
    {
        ContactEmail a = default;
        ContactEmail b = default;

        Assert.True(a == b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void ContactEmail_Default_NotEqualToNonDefault()
    {
        ContactEmail a = default;
        ContactEmail b = new EmailAddress("alice", "example.com");

        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void ContactEmail_Default_CompareTo_BothNull()
    {
        ContactEmail a = default;
        ContactEmail b = default;
        Assert.Equal(0, a.CompareTo(b));
    }

    [Fact]
    public void ContactEmail_Default_CompareTo_NullLessThanNonNull()
    {
        ContactEmail a = default;
        ContactEmail b = new EmailAddress("alice", "example.com");
        Assert.True(a.CompareTo(b) < 0);
    }

    [Fact]
    public void ContactEmail_Default_ComparisonOperators()
    {
        ContactEmail a = default;
        ContactEmail b = new EmailAddress("alice", "example.com");

        Assert.True(a < b);
        Assert.False(a > b);
        Assert.True(a <= b);
        Assert.False(a >= b);
    }

    // =========================================================================
    // Price — wraps Money (class with user-defined operators)
    // =========================================================================

    // --- Implicit Conversions ---

    [Fact]
    public void Price_ImplicitConversion_FromUnderlying()
    {
        var money = new Money(9.99m, "USD");
        Price price = money;
        Assert.Equal(9.99m, price.Amount);
        Assert.Equal("USD", price.Currency);
    }

    [Fact]
    public void Price_ImplicitConversion_ToUnderlying()
    {
        Price price = new Money(19.99m, "EUR");
        Money raw = price;
        Assert.Equal(19.99m, raw.Amount);
        Assert.Equal("EUR", raw.Currency);
    }

    [Fact]
    public void Price_RoundTrip()
    {
        var original = new Money(42.00m, "GBP");
        Price price = original;
        Money result = price;
        Assert.Equal(original, result);
    }

    // --- Arithmetic Operators ---

    [Fact]
    public void Price_Addition_PriceAndPrice()
    {
        Price a = new Money(10.00m, "USD");
        Price b = new Money(5.50m, "USD");

        Price result = a + b;
        Assert.Equal(15.50m, result.Amount);
    }

    [Fact]
    public void Price_Addition_PriceAndMoney()
    {
        Price price = new Money(10.00m, "USD");
        var tax = new Money(1.50m, "USD");

        Price result = price + tax;
        Assert.Equal(11.50m, result.Amount);
    }

    [Fact]
    public void Price_Subtraction_PriceAndPrice()
    {
        Price a = new Money(20.00m, "USD");
        Price b = new Money(7.50m, "USD");

        Price result = a - b;
        Assert.Equal(12.50m, result.Amount);
    }

    [Fact]
    public void Price_Subtraction_PriceAndMoney()
    {
        Price price = new Money(20.00m, "USD");
        var discount = new Money(3.00m, "USD");

        Price result = price - discount;
        Assert.Equal(17.00m, result.Amount);
    }

    [Fact]
    public void Price_Multiplication_PriceAndDecimal()
    {
        Price price = new Money(10.00m, "USD");

        Price doubled = price * 2.0m;
        Assert.Equal(20.00m, doubled.Amount);

        Price halfed = price * 0.5m;
        Assert.Equal(5.00m, halfed.Amount);
    }

    [Fact]
    public void Price_UnaryNegation()
    {
        Price price = new Money(15.00m, "USD");
        Price negated = -price;
        Assert.Equal(-15.00m, negated.Amount);
    }

    // --- Equality ---

    [Fact]
    public void Price_Equality_Equal()
    {
        Price a = new Money(9.99m, "USD");
        Price b = new Money(9.99m, "USD");

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
    }

    [Fact]
    public void Price_Equality_NotEqual_Amount()
    {
        Price a = new Money(9.99m, "USD");
        Price b = new Money(19.99m, "USD");

        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }

    [Fact]
    public void Price_Equality_NotEqual_Currency()
    {
        Price a = new Money(9.99m, "USD");
        Price b = new Money(9.99m, "EUR");

        Assert.NotEqual(a, b);
        Assert.True(a != b);
    }

    [Fact]
    public void Price_GetHashCode_ConsistentWithEquals()
    {
        Price a = new Money(42.00m, "USD");
        Price b = new Money(42.00m, "USD");
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // --- Property Forwarding ---

    [Fact]
    public void Price_PropertyForwarding_Amount()
    {
        Price price = new Money(29.95m, "USD");
        Assert.Equal(29.95m, price.Amount);
    }

    [Fact]
    public void Price_PropertyForwarding_Currency()
    {
        Price price = new Money(29.95m, "EUR");
        Assert.Equal("EUR", price.Currency);
    }

    // --- Method Forwarding with Return Type Wrapping ---

    [Fact]
    public void Price_WithAmount_ReturnsPrice()
    {
        Price price = new Money(10.00m, "USD");
        Price updated = price.WithAmount(25.00m);

        Assert.IsType<Price>(updated);
        Assert.Equal(25.00m, updated.Amount);
        Assert.Equal("USD", updated.Currency);
    }

    // --- ToString ---

    [Fact]
    public void Price_ToString()
    {
        Price price = new Money(9.99m, "USD");
        Assert.Equal("9.99 USD", price.ToString());
    }

    // --- Default (null underlying) ---

    [Fact]
    public void Price_Default_HasNullUnderlying()
    {
        Price price = default;
        Assert.Null(price.Value);
    }

    [Fact]
    public void Price_Default_ToStringReturnsEmpty()
    {
        Price price = default;
        Assert.Equal("", price.ToString());
    }

    [Fact]
    public void Price_Default_GetHashCodeDoesNotThrow()
    {
        Price price = default;
        Assert.Equal(0, price.GetHashCode());
    }

    [Fact]
    public void Price_Default_EqualsDefault()
    {
        Price a = default;
        Price b = default;

        Assert.True(a == b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Price_Default_NotEqualToNonDefault()
    {
        Price a = default;
        Price b = new Money(1.00m, "USD");

        Assert.False(a == b);
        Assert.True(a != b);
    }

    // --- Arithmetic Chain ---

    [Fact]
    public void Price_ArithmeticChain()
    {
        Price basePrice = new Money(100.00m, "USD");
        Price tax = new Money(8.50m, "USD");
        Price discount = new Money(10.00m, "USD");

        Price total = basePrice + tax - discount;
        Assert.Equal(98.50m, total.Amount);
    }

    // =========================================================================
    // Tint — wraps Rgb (record)
    // =========================================================================

    // --- Implicit Conversions ---

    [Fact]
    public void Tint_ImplicitConversion_FromUnderlying()
    {
        var rgb = new Rgb(255, 128, 0);
        Tint tint = rgb;
        Assert.Equal(255, tint.R);
        Assert.Equal(128, tint.G);
        Assert.Equal(0, tint.B);
    }

    [Fact]
    public void Tint_ImplicitConversion_ToUnderlying()
    {
        Tint tint = new Rgb(0, 255, 0);
        Rgb raw = tint;
        Assert.Equal(new Rgb(0, 255, 0), raw);
    }

    [Fact]
    public void Tint_RoundTrip()
    {
        var original = new Rgb(100, 150, 200);
        Tint tint = original;
        Rgb result = tint;
        Assert.Equal(original, result);
    }

    // --- Equality (uses record equality via object.Equals) ---

    [Fact]
    public void Tint_Equality_Equal()
    {
        Tint a = new Rgb(255, 0, 0);
        Tint b = new Rgb(255, 0, 0);

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Tint_Equality_NotEqual()
    {
        Tint a = new Rgb(255, 0, 0);
        Tint b = new Rgb(0, 255, 0);

        Assert.NotEqual(a, b);
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void Tint_GetHashCode_ConsistentWithEquals()
    {
        Tint a = new Rgb(128, 128, 128);
        Tint b = new Rgb(128, 128, 128);
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Tint_GetHashCode_MatchesUnderlying()
    {
        var rgb = new Rgb(42, 84, 126);
        Tint tint = rgb;
        Assert.Equal(rgb.GetHashCode(), tint.GetHashCode());
    }

    // --- Property Forwarding ---

    [Fact]
    public void Tint_PropertyForwarding_R()
    {
        Tint tint = new Rgb(10, 20, 30);
        Assert.Equal(10, tint.R);
    }

    [Fact]
    public void Tint_PropertyForwarding_G()
    {
        Tint tint = new Rgb(10, 20, 30);
        Assert.Equal(20, tint.G);
    }

    [Fact]
    public void Tint_PropertyForwarding_B()
    {
        Tint tint = new Rgb(10, 20, 30);
        Assert.Equal(30, tint.B);
    }

    // --- Method Forwarding with Return Type Wrapping ---

    [Fact]
    public void Tint_Invert_ReturnsTint()
    {
        Tint tint = new Rgb(255, 0, 128);
        Tint inverted = tint.Invert();

        Assert.IsType<Tint>(inverted);
        Assert.Equal(0, inverted.R);
        Assert.Equal(255, inverted.G);
        Assert.Equal(127, inverted.B);
    }

    [Fact]
    public void Tint_Mix_ReturnsTint()
    {
        Tint a = new Rgb(200, 100, 0);
        Tint b = new Rgb(100, 200, 100);

        Tint mixed = a.Mix(b);
        Assert.IsType<Tint>(mixed);
        Assert.Equal(150, mixed.R);
        Assert.Equal(150, mixed.G);
        Assert.Equal(50, mixed.B);
    }

    [Fact]
    public void Tint_Mix_TakesAliasTypeParameter()
    {
        // Mix parameter is Tint (not Rgb) — verifies parameter type wrapping
        Tint a = new Rgb(0, 0, 0);
        Tint b = new Rgb(254, 254, 254);

        Tint mixed = a.Mix(b);
        Assert.Equal(127, mixed.R);
        Assert.Equal(127, mixed.G);
        Assert.Equal(127, mixed.B);
    }

    // --- Deconstruct ---

    [Fact]
    public void Tint_Deconstruct()
    {
        Tint tint = new Rgb(10, 20, 30);
        var (r, g, b) = tint;

        Assert.Equal(10, r);
        Assert.Equal(20, g);
        Assert.Equal(30, b);
    }

    // --- ToString ---

    [Fact]
    public void Tint_ToString()
    {
        Tint tint = new Rgb(255, 128, 0);
        Assert.Equal("#FF8000", tint.ToString());
    }

    [Fact]
    public void Tint_ToString_Black()
    {
        Tint tint = new Rgb(0, 0, 0);
        Assert.Equal("#000000", tint.ToString());
    }

    // --- Default (null underlying) ---

    [Fact]
    public void Tint_Default_HasNullUnderlying()
    {
        Tint tint = default;
        Assert.Null(tint.Value);
    }

    [Fact]
    public void Tint_Default_ToStringReturnsEmpty()
    {
        Tint tint = default;
        Assert.Equal("", tint.ToString());
    }

    [Fact]
    public void Tint_Default_GetHashCodeDoesNotThrow()
    {
        Tint tint = default;
        Assert.Equal(0, tint.GetHashCode());
    }

    [Fact]
    public void Tint_Default_EqualsDefault()
    {
        Tint a = default;
        Tint b = default;

        Assert.True(a == b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Tint_Default_NotEqualToNonDefault()
    {
        Tint a = default;
        Tint b = new Rgb(1, 2, 3);

        Assert.False(a == b);
        Assert.True(a != b);
    }

    // =========================================================================
    // Cross-cutting: Ref patterns, array access, collections
    // =========================================================================

    [Fact]
    public void ContactEmail_RefLocal_ArrayAccess()
    {
        var emails = new ContactEmail[]
        {
            new EmailAddress("alice", "example.com"),
            new EmailAddress("bob", "example.com"),
        };

        ref var email = ref emails[0];
        email = new EmailAddress("charlie", "example.com");

        Assert.Equal("charlie", emails[0].User);
    }

    [Fact]
    public void Price_RefLocal_ArrayAccess()
    {
        var prices = new Price[]
        {
            new Money(10.00m, "USD"),
            new Money(20.00m, "USD"),
        };

        ref var price = ref prices[1];
        price = new Money(99.99m, "USD");

        Assert.Equal(99.99m, prices[1].Amount);
    }

    [Fact]
    public void Tint_RefLocal_ArrayAccess()
    {
        var tints = new Tint[]
        {
            new Rgb(255, 0, 0),
            new Rgb(0, 255, 0),
            new Rgb(0, 0, 255),
        };

        ref var tint = ref tints[2];
        tint = new Rgb(128, 128, 128);

        Assert.Equal(128, tints[2].R);
    }

    [Fact]
    public void ContactEmail_UsableAsDictionaryKey()
    {
        var lookup = new Dictionary<ContactEmail, string>();

        ContactEmail key = new EmailAddress("alice", "example.com");
        lookup[key] = "Alice Smith";

        ContactEmail sameKey = new EmailAddress("alice", "example.com");
        Assert.Equal("Alice Smith", lookup[sameKey]);
    }

    [Fact]
    public void Tint_UsableInHashSet()
    {
        var set = new HashSet<Tint>
        {
            new Rgb(255, 0, 0),
            new Rgb(0, 255, 0),
            new Rgb(255, 0, 0), // duplicate
        };

        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void ContactEmail_SortableViaComparison()
    {
        var emails = new ContactEmail[]
        {
            new EmailAddress("charlie", "example.com"),
            new EmailAddress("alice", "example.com"),
            new EmailAddress("bob", "example.com"),
        };

        Array.Sort(emails);

        Assert.Equal("alice", emails[0].User);
        Assert.Equal("bob", emails[1].User);
        Assert.Equal("charlie", emails[2].User);
    }
}