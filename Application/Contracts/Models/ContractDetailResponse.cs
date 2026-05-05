namespace Application.Contracts.Models;

public class ContractDetailResponse
{
    public long Id { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public DateOnly ContractDate { get; set; }
    public string ContractType { get; set; } = string.Empty;
    public long? BuyerId { get; set; }
    public string? BuyerName { get; set; }
    public long? SupplierId { get; set; }
    public string? SupplierName { get; set; }
    public long? ImportContractId { get; set; }
    public string? ImportContractNo { get; set; }
    public long? SalesOrderId { get; set; }
    public string? Incoterm { get; set; }
    public string? PaymentTerms { get; set; }
    public string? ShipmentDatePlan { get; set; }
    public string? Note { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public List<ContractAttachmentResponse> Attachments { get; set; } = new();
    public List<ContractPackageResponse> Packages { get; set; } = new();
    public List<ImportContractItemResponse> Items { get; set; } = new();
}
