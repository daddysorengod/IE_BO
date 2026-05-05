namespace Application.Partners.Repositories;

public class PartnerSearchCondition
{
    public string? Keyword { get; set; }
    public string? PartnerType { get; set; }
    public bool? IsActive { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}
