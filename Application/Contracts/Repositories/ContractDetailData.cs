using Domain.Contracts;

namespace Application.Contracts.Repositories;

public class ContractDetailData
{
    public Contract Contract { get; set; } = new();
    public IReadOnlyList<ContractAttachment> Attachments { get; set; } = Array.Empty<ContractAttachment>();
    public IReadOnlyList<ContractPackage> Packages { get; set; } = Array.Empty<ContractPackage>();
    public IReadOnlyList<ContractItem> Items { get; set; } = Array.Empty<ContractItem>();
}
