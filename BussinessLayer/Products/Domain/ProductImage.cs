namespace BussinessLayer.Products.Domain;

public class ProductImage
{
    public long ProductId { get; set; }
    public long Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    //public string IsDefault { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? IsDefault { get; set; } 
    public string Delt { get; set; } = string.Empty;
}
