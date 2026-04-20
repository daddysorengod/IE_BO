namespace Application.Products.Models;

public class SearchProductRequest
{
    public string? Keyword { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? HsCode { get; set; }
    public string? UnitOfMeasure { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
