namespace Application.Contracts.Models;

public class ContractPackageItemResponse
{
    public long Id { get; set; }
    public long PackageId { get; set; }
    public long ProductId { get; set; }
    public long? SupplierId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal SellPrice { get; set; }
    public decimal TotalImportAmount { get; set; }
    public decimal TotalSellAmount { get; set; }
    public decimal? TotalCtns { get; set; }
    public decimal? TotalCbm { get; set; }
}
