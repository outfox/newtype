namespace newtype.tests;

/// <summary>
/// A class with properties, IEquatable, and IComparable.
/// Tests property forwarding, comparison operators, and null safety.
/// </summary>
public class EmailAddress : IEquatable<EmailAddress>, IComparable<EmailAddress>
{
    public string User { get; }
    public string Domain { get; }

    public EmailAddress(string user, string domain)
    {
        User = user;
        Domain = domain;
    }

    public EmailAddress WithDomain(string domain) => new(User, domain);

    public override string ToString() => $"{User}@{Domain}";

    public bool Equals(EmailAddress? other) =>
        other is not null && User == other.User && Domain == other.Domain;

    public override bool Equals(object? obj) => obj is EmailAddress other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(User, Domain);

    public int CompareTo(EmailAddress? other) =>
        other is null ? 1 : string.Compare(ToString(), other.ToString(), StringComparison.Ordinal);
}

/// <summary>
/// A class with user-defined arithmetic and unary operators.
/// Tests operator forwarding on reference types.
/// </summary>
public class Money : IEquatable<Money>
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public static Money operator +(Money a, Money b) => new(a.Amount + b.Amount, a.Currency);
    public static Money operator -(Money a, Money b) => new(a.Amount - b.Amount, a.Currency);
    public static Money operator *(Money a, decimal factor) => new(a.Amount * factor, a.Currency);
    public static Money operator -(Money a) => new(-a.Amount, a.Currency);

    public Money WithAmount(decimal amount) => new(amount, Currency);

    public override string ToString() => $"{Amount} {Currency}";

    public bool Equals(Money? other) =>
        other is not null && Amount == other.Amount && Currency == other.Currency;

    public override bool Equals(object? obj) => obj is Money other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Amount, Currency);
}

/// <summary>
/// A record with methods that return the same type.
/// Tests record wrapping, built-in equality, and return type wrapping.
/// </summary>
public record Rgb(byte R, byte G, byte B)
{
    public Rgb Invert() => new((byte)(255 - R), (byte)(255 - G), (byte)(255 - B));

    public Rgb Mix(Rgb other) => new(
        (byte)((R + other.R) / 2),
        (byte)((G + other.G) / 2),
        (byte)((B + other.B) / 2));

    public override string ToString() => $"#{R:X2}{G:X2}{B:X2}";
}
