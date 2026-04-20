namespace Application.Products.Repositories;

public class ProductSearchResult
{
    public IReadOnlyList<ProductSearchData> Items { get; set; } = Array.Empty<ProductSearchData>();
    public int Total { get; set; }
}
