using BussinessLayer.Products.Domain;

namespace BussinessLayer.Products.Repositories;

public class ProductSearchResult
{
    public IReadOnlyList<Product> Items { get; set; } = Array.Empty<Product>();
    public int Total { get; set; }
}
