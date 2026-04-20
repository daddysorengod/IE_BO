namespace Domain.Products;

public class ProductImage
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? IsDefault { get; set; }
    public int SortOrder { get; set; }
}
