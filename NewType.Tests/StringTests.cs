using Xunit;

namespace newtype.tests;

public class StringTests
{
    // --- Implicit Conversions ---

    [Fact]
    public void Name_ImplicitConversion_FromString()
    {
        Name name = "Alice";
        Assert.Equal("Alice", name.Value);
    }

    [Fact]
    public void Name_ImplicitConversion_ToString()
    {
        Name name = "Bob";
        string raw = name;
        Assert.Equal("Bob", raw);
    }

    [Fact]
    public void Name_RoundTrip()
    {
        var original = "Charlie";
        Name name = original;
        string result = name;
        Assert.Equal(original, result);
    }

    // --- Equality ---

    [Fact]
    public void Name_Equality_Equal()
    {
        Name a = "hello";
        Name b = "hello";

        Assert.Equal(a, b);
        Assert.True(a == b);
        Assert.False(a != b);
        Assert.True(a.Equals(b));
    }

    [Fact]
    public void Name_Equality_NotEqual()
    {
        Name a = "hello";
        Name b = "world";

        Assert.NotEqual(a, b);
        Assert.False(a == b);
        Assert.True(a != b);
    }

    [Fact]
    public void Name_GetHashCode_MatchesUnderlying()
    {
        Name name = "test";
        Assert.Equal("test".GetHashCode(), name.GetHashCode());
    }

    [Fact]
    public void Name_GetHashCode_ConsistentWithEquals()
    {
        Name a = "same";
        Name b = "same";

        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    // --- String Concatenation ---

    [Fact]
    public void Name_Concat_NamePlusName()
    {
        Name first = "Hello";
        Name second = " World";

        Name result = first + second;
        Assert.Equal("Hello World", (string)result);
    }

    [Fact]
    public void Name_Concat_NamePlusString()
    {
        Name name = "Hello";

        Name result = name + " World";
        Assert.Equal("Hello World", (string)result);
    }

    [Fact]
    public void Name_Concat_StringPlusName()
    {
        Name name = "World";

        Name result = "Hello " + name;
        Assert.Equal("Hello World", (string)result);
    }

    [Fact]
    public void Name_Concat_MixedPatterns()
    {
        Name name = "Alice";

        Name greeting = "Hello, " + name;
        Assert.Equal("Hello, Alice", (string)greeting);

        Name suffixed = name + "!";
        Assert.Equal("Alice!", (string)suffixed);
    }

    // --- Comparison Operators ---

    [Fact]
    public void Name_Comparison_LessThan()
    {
        Name a = "apple";
        Name b = "banana";

        Assert.True(a < b);
        Assert.False(b < a);
    }

    [Fact]
    public void Name_Comparison_GreaterThan()
    {
        Name a = "banana";
        Name b = "apple";

        Assert.True(a > b);
        Assert.False(b > a);
    }

    [Fact]
    public void Name_Comparison_LessThanOrEqual()
    {
        Name a = "apple";
        Name b = "banana";
        Name c = "apple";

        Assert.True(a <= b);
        Assert.True(a <= c);
        Assert.False(b <= a);
    }

    [Fact]
    public void Name_Comparison_GreaterThanOrEqual()
    {
        Name a = "banana";
        Name b = "apple";
        Name c = "banana";

        Assert.True(a >= b);
        Assert.True(a >= c);
        Assert.False(b >= a);
    }

    // --- CompareTo ---

    [Fact]
    public void Name_CompareTo()
    {
        Name a = "apple";
        Name b = "banana";
        Name c = "apple";

        Assert.True(a.CompareTo(b) < 0);
        Assert.True(b.CompareTo(a) > 0);
        Assert.Equal(0, a.CompareTo(c));
    }

    // --- Instance Properties ---

    [Fact]
    public void Name_Length()
    {
        Name name = "Hello";
        Assert.Equal(5, name.Length);
    }

    // --- Instance Methods ---

    [Fact]
    public void Name_Contains()
    {
        Name name = "Hello World";
        Assert.True(name.Contains("World"));
        Assert.False(name.Contains("xyz"));
    }

    [Fact]
    public void Name_StartsWith()
    {
        Name name = "Hello World";
        Assert.True(name.StartsWith("Hello"));
        Assert.False(name.StartsWith("World"));
    }

    [Fact]
    public void Name_EndsWith()
    {
        Name name = "Hello World";
        Assert.True(name.EndsWith("World"));
        Assert.False(name.EndsWith("Hello"));
    }

    [Fact]
    public void Name_Substring()
    {
        Name name = "Hello World";
        Name sub = name.Substring(6);
        Assert.Equal("World", (string)sub);
    }

    [Fact]
    public void Name_Substring_WithLength()
    {
        Name name = "Hello World";
        Name sub = name.Substring(0, 5);
        Assert.Equal("Hello", (string)sub);
    }

    [Fact]
    public void Name_ToUpper()
    {
        Name name = "hello";
        Name upper = name.ToUpper();
        Assert.Equal("HELLO", (string)upper);
    }

    [Fact]
    public void Name_ToLower()
    {
        Name name = "HELLO";
        Name lower = name.ToLower();
        Assert.Equal("hello", (string)lower);
    }

    [Fact]
    public void Name_Trim()
    {
        Name name = "  hello  ";
        Name trimmed = name.Trim();
        Assert.Equal("hello", (string)trimmed);
    }

    [Fact]
    public void Name_Replace()
    {
        Name name = "Hello World";
        Name replaced = name.Replace("World", "There");
        Assert.Equal("Hello There", (string)replaced);
    }

    [Fact]
    public void Name_Insert()
    {
        Name name = "HelloWorld";
        Name inserted = name.Insert(5, " ");
        Assert.Equal("Hello World", (string)inserted);
    }

    // --- Return Type Wrapping ---

    [Fact]
    public void Name_ReturnType_SubstringReturnsName()
    {
        Name name = "Hello";
        // This should return Name, not string
        Name result = name.Substring(0, 3);
        Assert.IsType<Name>(result);
        Assert.Equal("Hel", (string)result);
    }

    [Fact]
    public void Name_ReturnType_ToUpperReturnsName()
    {
        Name name = "hello";
        Name result = name.ToUpper();
        Assert.IsType<Name>(result);
    }

    [Fact]
    public void Name_ReturnType_ReplaceReturnsName()
    {
        Name name = "aaa";
        Name result = name.Replace("a", "b");
        Assert.IsType<Name>(result);
        Assert.Equal("bbb", (string)result);
    }

    // --- ToString ---

    [Fact]
    public void Name_ToString()
    {
        Name name = "Alice";
        Assert.Equal("Alice", name.ToString());
    }

    // --- Default Value (null underlying) ---

    [Fact]
    public void Name_Default_HasNullUnderlying()
    {
        Name name = default;
        Assert.Null(name.Value);
    }

    [Fact]
    public void Name_Default_ToStringReturnsEmpty()
    {
        Name name = default;
        Assert.Equal("", name.ToString());
    }

    [Fact]
    public void Name_Default_GetHashCodeDoesNotThrow()
    {
        Name name = default;
        Assert.Equal(0, name.GetHashCode());
    }

    [Fact]
    public void Name_Default_EqualsDefault()
    {
        Name a = default;
        Name b = default;

        Assert.True(a == b);
        Assert.True(a.Equals(b));
        Assert.Equal(a, b);
    }

    [Fact]
    public void Name_Default_NotEqualToNonDefault()
    {
        Name a = default;
        Name b = "hello";

        Assert.False(a == b);
        Assert.True(a != b);
    }

    // --- Ref Patterns ---

    [Fact]
    public void Name_RefLocal_ArrayAccess()
    {
        var names = new Name[] { "Alice", "Bob", "Charlie" };

        ref var name = ref names[1];
        name = "Dave";

        Assert.Equal("Dave", (string)names[1]);
    }

    [Fact]
    public void Name_Array_Iteration()
    {
        var names = new Name[] { "Alice", "Bob", "Charlie" };

        var result = new List<string>();
        foreach (var name in names)
        {
            result.Add(name.ToUpper());
        }

        Assert.Equal(new[] { "ALICE", "BOB", "CHARLIE" }, result);
    }
}
