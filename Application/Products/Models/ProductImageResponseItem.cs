namespace Application.Products.Models;

public class ProductImageResponseItem
{
    public long ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? IsDefault { get; set; }
    public int SortOrder { get; set; }
}
