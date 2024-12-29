using DataEntities;

namespace Store.Services
{
    public class BasketService
    {
        // In-memory storage for basket items (usually would be saved in a session, database, etc.)
        private readonly List<BasketItem> basket = new();

        // Adds an item to the basket, or updates quantity if item already exists
        public async Task AddAsync(BasketItem basketItem)
        {
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

            await Task.CompletedTask;
        }

        // Retrieves all items in the basket
        public async Task<List<BasketItem>> GetBasketItemsAsync()
        {
            // Return the list of items in the basket
            return await Task.FromResult(basket);
        }

        // Clears the basket
        public void ClearBasket()
        {
            basket.Clear();
        }

        // Updates the quantity of a specific item in the basket
        public async Task UpdateQuantityAsync(int productId, int quantity)
        {
            var item = basket.FirstOrDefault(b => b.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
            }

            await Task.CompletedTask;
        }

        // Removes an item from the basket
        public async Task RemoveAsync(int productId)
        {
            var item = basket.FirstOrDefault(b => b.ProductId == productId);
            if (item != null)
            {
                basket.Remove(item);
            }

            await Task.CompletedTask;
        }
    }

    // BasketItem represents a product added to the basket
    public class BasketItem
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string PictureUrl { get; set; }
    }
}