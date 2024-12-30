using DataEntities;
using Store.WebAppComponents;
using System.Text.Json;

namespace Store.Services;

public class ProductService(HttpClient httpClient)
{
    private readonly string remoteServiceBaseUrl = "/api/Product";

    public async Task<List<Product>> GetProducts()
    {
        List<Product>? products = null;
        var response = await httpClient.GetAsync(remoteServiceBaseUrl);
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            products = await response.Content.ReadFromJsonAsync(ProductSerializerContext.Default.ListProduct);
        }

        return products ?? new List<Product>();
    }

    public async Task<List<Product>> GetProductsByUserId(Guid id)
    {
        List<Product>? products = null;
        var response = await httpClient.GetAsync($"/api/Product/user/{id}");
        if (response.IsSuccessStatusCode)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            products = await response.Content.ReadFromJsonAsync(ProductSerializerContext.Default.ListProduct);
        }

        return products ?? new List<Product>();
    }

    public async Task<CatalogResult> GetAllCatalogItems()
    {
        var items = await httpClient.GetFromJsonAsync<List<Item>>(remoteServiceBaseUrl);
        return new CatalogResult(items ?? new List<Item>());
    }

    public Task<Item?> GetCatalogItem(Guid id)
    {
        var uri = $"{remoteServiceBaseUrl}/{id}";
        return httpClient.GetFromJsonAsync<Item>(uri);
    }

    public async Task<bool> CreateProductAsync(Product product)
    {
        // Generate a new GUID for the Product (if necessary)
        product.Id = Guid.NewGuid(); // Ensure the Product gets a unique ID

        var productJson = JsonSerializer.Serialize(product);
        Console.WriteLine($"Serialized Product: {productJson}");

        var response = await httpClient.PostAsJsonAsync($"{remoteServiceBaseUrl}", product);

        if (response.IsSuccessStatusCode)
        {
            return true; // Successfully created the product
        }

        return false; // Failed to create the product
    }
}