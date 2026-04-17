namespace BussinessLayer.Products.Models;

public class SearchProductResponse
{
    public List<SearchProductItem> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
