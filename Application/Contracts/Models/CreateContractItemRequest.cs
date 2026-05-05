namespace Application.Contracts.Models;

public class CreateContractItemRequest
{
    public long ProductId { get; set; }
    public long SupplierId { get; set; }
    public decimal Quantity { get; set; }
    public decimal ImportPrice { get; set; }
    public decimal SellPrice { get; set; }
}
