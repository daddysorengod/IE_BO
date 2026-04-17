namespace BussinessLayer.Products.Models;

public class ProductImageUpsertRequest
{
    public long ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string IsDefault { get; set; } = "N";
    public int SortOrder { get; set; }
}