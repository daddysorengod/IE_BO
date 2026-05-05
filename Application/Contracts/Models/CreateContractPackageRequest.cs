namespace Application.Contracts.Models;

public class CreateContractPackageRequest
{
    public long PackageId { get; set; }
    public long ProductId { get; set; }
    public decimal PackageQuantity { get; set; } = 1;
    public decimal SellPrice { get; set; }
    public string? Note { get; set; }
}
