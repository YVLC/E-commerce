using DataEntities;

namespace Store.Services
{
    public class BasketService
    {
        private readonly List<Product> basket = new();

        public void AddToBasket(Product product)
        {
            basket.Add(product);
        }

        public List<Product> GetBasket()
        {
            return basket;
        }

        public void ClearBasket()
        {
            basket.Clear();
        }
    }
}