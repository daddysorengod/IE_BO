namespace Infrastructure.Persistence.Entities.Contracts;

internal class ContractItem
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long? ContractPackageId { get; set; }
    public long? PackageId { get; set; }
    public long ProductId { get; set; }
    public long? SupplierId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal SellPrice { get; set; }
    public decimal TotalImportAmount { get; set; }
    public decimal TotalSellAmount { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
