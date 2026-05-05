namespace Application.Contracts.Models;

public class CreateImportContractRequest
{
    public string ContractNo { get; set; } = string.Empty;
    public DateOnly ContractDate { get; set; }
    public long SupplierId { get; set; }
    public string? Incoterm { get; set; }
    public string? PaymentTerms { get; set; }
    public string? ShipmentDatePlan { get; set; }
    public string? Note { get; set; }
    public List<CreateContractAttachmentRequest> Attachments { get; set; } = new();
    public List<CreateImportContractItemRequest> Items { get; set; } = new();
}
