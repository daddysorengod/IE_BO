namespace Application.Contracts.Models;

public class CreateContractRequest
{
    public string ContractNo { get; set; } = string.Empty;
    public DateOnly ContractDate { get; set; }
    public long BuyerId { get; set; }
    public string ImportContractNo { get; set; } = string.Empty;
    public string? Incoterm { get; set; }
    public string? PaymentTerms { get; set; }
    public string? ShipmentDatePlan { get; set; }
    public string? Note { get; set; }
    public List<CreateContractAttachmentRequest> Attachments { get; set; } = new();
    public List<CreateContractPackageRequest> Packages { get; set; } = new();
}
