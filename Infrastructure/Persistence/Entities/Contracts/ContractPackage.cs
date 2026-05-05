namespace Infrastructure.Persistence.Entities.Contracts;

internal class ContractPackage
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long PackageId { get; set; }
    public long ProductId { get; set; }
    public decimal PackageQuantity { get; set; }
    public string? Note { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
