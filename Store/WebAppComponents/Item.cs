namespace Store.WebAppComponents
{
    public record Item(
        int Id,
        string Name,
        string Description,
        decimal Price,
        string PictureUrl);
    public record CatalogResult(List<Item> Data);
}
