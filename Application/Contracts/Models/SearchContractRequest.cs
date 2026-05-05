namespace Application.Contracts.Models;

public class SearchContractRequest
{
    public string? Keyword { get; set; }
    public string? ContractType { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
