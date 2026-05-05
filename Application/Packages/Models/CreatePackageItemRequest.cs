namespace Application.Packages.Models;

public class CreatePackageItemRequest
{
    public long ProductId { get; set; }
    public long SupplierId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal SellPrice { get; set; }
    public decimal? TotalCtns { get; set; }
    public decimal? TotalCbm { get; set; }
}
