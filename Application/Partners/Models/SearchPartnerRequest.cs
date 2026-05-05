namespace Application.Partners.Models;

public class SearchPartnerRequest
{
    public string? Keyword { get; set; }
    public string? PartnerType { get; set; }
    public bool? IsActive { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
