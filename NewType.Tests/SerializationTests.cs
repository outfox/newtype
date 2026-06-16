using System.ComponentModel;
using System.Text.Json;
using Xunit;

namespace newtype.tests;

/// <summary>
/// Issue #4 — newtypes opted into <c>NewtypeOptions.Serializable</c> round-trip through
/// System.Text.Json (via a generated JsonConverter) and TypeConverter-based serializers.
/// </summary>
public class SerializationTests
{
    // --- System.Text.Json ---

    [Fact]
    public void Json_String_SerializesAsBareValue()
    {
        var json = JsonSerializer.Serialize(new SerialId("CTR-002"));
        Assert.Equal("\"CTR-002\"", json);
    }

    [Fact]
    public void Json_String_RoundTrips()
    {
        var original = new SerialId("CTR-002");
        var json = JsonSerializer.Serialize(original);
        var restored = JsonSerializer.Deserialize<SerialId>(json);

        Assert.Equal(original, restored);
        Assert.Equal("CTR-002", restored.Value);
    }

    [Fact]
    public void Json_Int_SerializesAsBareNumber()
    {
        var json = JsonSerializer.Serialize(new SerialCount(42));
        Assert.Equal("42", json);

        var restored = JsonSerializer.Deserialize<SerialCount>(json);
        Assert.Equal(42, restored.Value);
    }

    [Fact]
    public void Json_AsPropertyOnContainingObject()
    {
        // Mirrors issue #4: a newtype used as a property serializes its underlying value
        // rather than an empty element.
        var order = new Order { Contract = new SerialId("CTR-002") };

        var json = JsonSerializer.Serialize(order);
        Assert.Equal("{\"Contract\":\"CTR-002\"}", json);

        var restored = JsonSerializer.Deserialize<Order>(json);
        Assert.NotNull(restored);
        Assert.Equal("CTR-002", restored!.Contract.Value);
    }

    private sealed class Order
    {
        public SerialId Contract { get; set; }
    }

    // --- TypeConverter (Newtonsoft.Json, ASP.NET model binding, configuration binding, ...) ---

    [Fact]
    public void TypeConverter_IsDiscoverable()
    {
        var converter = TypeDescriptor.GetConverter(typeof(SerialId));
        Assert.IsType<SerialId.NewtypeTypeConverter>(converter);
    }

    [Fact]
    public void TypeConverter_String_RoundTrips()
    {
        var converter = TypeDescriptor.GetConverter(typeof(SerialId));

        Assert.True(converter.CanConvertFrom(typeof(string)));
        Assert.True(converter.CanConvertTo(typeof(string)));

        var text = converter.ConvertToString(new SerialId("abc"));
        Assert.Equal("abc", text);

        var value = converter.ConvertFromString("abc");
        Assert.Equal(new SerialId("abc"), value);
    }

    [Fact]
    public void TypeConverter_Int_RoundTrips()
    {
        var converter = TypeDescriptor.GetConverter(typeof(SerialCount));

        var text = converter.ConvertToString(new SerialCount(7));
        Assert.Equal("7", text);

        var value = converter.ConvertFromString("7");
        Assert.Equal(new SerialCount(7), value);
    }

    [Fact]
    public void NonSerializable_Newtype_HasNoCustomConverter()
    {
        // Name is a plain [newtype<string>] without the Serializable flag, so no [TypeConverter]
        // is generated and TypeDescriptor falls back to the default base converter. Any generated
        // converter would derive from TypeConverter, so this exact-type check enforces the gate.
        var converter = TypeDescriptor.GetConverter(typeof(Name));
        Assert.IsType<TypeConverter>(converter);
    }
}
