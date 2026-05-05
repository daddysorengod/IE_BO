namespace Application.Contracts.Models;

public class CreateContractAttachmentRequest
{
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileExtension { get; set; }
}
