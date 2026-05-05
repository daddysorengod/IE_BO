namespace Application.Contracts.Models;

public class CreateImportContractItemRequest
{
    public long ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ImportPrice { get; set; }
}
