namespace Store.WebAppComponents
{
    public record Item(
        int Id,
        string Name,
        string Description,
        decimal Price);
    public record CatalogResult(List<Item> Data);
}
