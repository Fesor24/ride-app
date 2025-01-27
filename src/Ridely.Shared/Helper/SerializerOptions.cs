using System.Text.Json;

namespace Ridely.Shared.Helper;
public static class SerializerOptions
{
    public static readonly JsonSerializerOptions Read = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    public static readonly JsonSerializerOptions Write = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}

public static class Serialize
{
    public static string Object<TValue>(TValue value) =>
        JsonSerializer.Serialize(value, SerializerOptions.Write);
}
