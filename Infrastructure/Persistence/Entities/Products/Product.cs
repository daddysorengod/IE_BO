namespace Infrastructure.Persistence.Entities.Products;

internal class Product
{
    public long Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? Delt { get; set; }
}
