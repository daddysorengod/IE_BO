using Domain.Common;

namespace Domain.Packages;

public class PackageItem : BaseEntity
{
    public long Id { get; set; }
    public long PackageId { get; set; }
    public long ProductId { get; set; }
    public long SupplierId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal SellPrice { get; set; }
    public decimal TotalImportAmount { get; set; }
    public decimal TotalSellAmount { get; set; }
    public decimal? TotalCtns { get; set; }
    public decimal? TotalCbm { get; set; }
}
