using Domain.Common;

namespace Domain.Contracts;

public class Contract : BaseEntity
{
    public long Id { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public DateOnly ContractDate { get; set; }
    public string ContractType { get; set; } = ContractTypes.Export;
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
}
