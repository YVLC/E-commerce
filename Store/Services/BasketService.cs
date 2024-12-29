using Microsoft.AspNetCore.Http;
using System.Text.Json;

public class BasketService
{
    private const string BasketSessionKey = "Basket";
    private readonly ISession session;

    public BasketService(IHttpContextAccessor httpContextAccessor)
    {
        session = httpContextAccessor.HttpContext?.Session ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    // Adds an item to the basket, or updates quantity if item already exists
    public async Task AddAsync(BasketItem basketItem)
    {
        var basket = await GetBasketItemsAsync();
        var existingItem = basket.FirstOrDefault(b => b.ProductId == basketItem.ProductId);

        if (existingItem != null)
        {
            // Update the quantity of the existing item
            existingItem.Quantity++;
        }
        else
        {
            // Add a new item to the basket
            basket.Add(basketItem);
        }

        SaveBasketToSession(basket);
        await Task.CompletedTask;
    }

    // Retrieves all items in the basket
    public async Task<List<BasketItem>> GetBasketItemsAsync()
    {
        var basketJson = session.GetString(BasketSessionKey);
        return basketJson is null
            ? new List<BasketItem>()
            : JsonSerializer.Deserialize<List<BasketItem>>(basketJson) ?? new List<BasketItem>();
    }

    // Clears the basket
    public void ClearBasket()
    {
        session.Remove(BasketSessionKey);
    }

    // Updates the quantity of a specific item in the basket
    public async Task UpdateQuantityAsync(Guid productId, int quantity)
    {
        var basket = await GetBasketItemsAsync();
        var item = basket.FirstOrDefault(b => b.ProductId == productId);

        if (item != null)
        {
            item.Quantity = quantity;
            SaveBasketToSession(basket);
        }

        await Task.CompletedTask;
    }

    // Removes an item from the basket
    public async Task RemoveAsync(Guid productId)
    {
        var basket = await GetBasketItemsAsync();
        var item = basket.FirstOrDefault(b => b.ProductId == productId);

        if (item != null)
        {
            basket.Remove(item);
            SaveBasketToSession(basket);
        }

        await Task.CompletedTask;
    }

    // Saves the basket to session storage
    private void SaveBasketToSession(List<BasketItem> basket)
    {
        var basketJson = JsonSerializer.Serialize(basket);
        session.SetString(BasketSessionKey, basketJson);
    }

    public async Task ClearBasketAsync()
    {
        if (session == null)
        {
            throw new InvalidOperationException("Session is not available.");
        }

        session.Remove(BasketSessionKey);
        await Task.CompletedTask;
    }
}

// BasketItem represents a product added to the basket
public class BasketItem
{
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }
}