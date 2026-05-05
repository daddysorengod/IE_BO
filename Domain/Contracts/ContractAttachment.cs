using Domain.Common;

namespace Domain.Contracts;

public class ContractAttachment : BaseEntity
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileExtension { get; set; }
}
