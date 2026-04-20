namespace Application.Products.Repositories;

public class ProductSearchCondition
{
    public string? Keyword { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? HsCode { get; set; }
    public string? UnitOfMeasure { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}
