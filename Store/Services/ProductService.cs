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
        var response = await httpClient.GetAsync($"/api/Product/{id}");
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

}