namespace Application.Contracts.Models;

public class ContractAttachmentResponse
{
    public long Id { get; set; }
    public long ContractId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileExtension { get; set; }
}
