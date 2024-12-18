namespace Store.WebAppComponents
{
    public static class ItemHelper
    {
        public static string Url(Item item)
            => $"item/{item.Id}";
    }
}
