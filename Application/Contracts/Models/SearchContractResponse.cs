namespace Application.Contracts.Models;

public class SearchContractResponse
{
    public List<SearchContractItem> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}
