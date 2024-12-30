using System.Text.Json.Serialization;

namespace DataEntities;

public class Product
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("Userid")]
    public Guid UserId { get; set; }

    [JsonPropertyName("Name")]
    public string? Name { get; set; }

    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    [JsonPropertyName("Price")]
    public decimal Price { get; set; }

    [JsonPropertyName("ImageUrl")]
    public string? ImageUrl { get; set; }

    [JsonPropertyName("Tag")]
    public string? Tag { get; set; }

    [JsonPropertyName("Type")]
    public string? Type { get; set; }
}


[JsonSerializable(typeof(List<Product>))]
public sealed partial class ProductSerializerContext : JsonSerializerContext
{
}