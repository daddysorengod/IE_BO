namespace Application.Contracts.Models;

public class ImportContractItemResponse
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public long ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public decimal Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal TotalImportAmount { get; set; }
}
