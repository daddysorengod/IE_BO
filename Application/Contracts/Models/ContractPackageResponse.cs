namespace Application.Contracts.Models;

public class ContractPackageResponse
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long PackageId { get; set; }
    public string? PackageCode { get; set; }
    public string? PackageName { get; set; }
    public long ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public decimal ProductQuantity { get; set; }
    public decimal PackageQuantity { get; set; }
    public decimal Quantity { get; set; }
    public decimal SellPrice { get; set; }
    public decimal TotalSellAmount { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? Note { get; set; }
}
