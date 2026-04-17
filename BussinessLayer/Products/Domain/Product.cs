namespace BussinessLayer.Products.Domain;

public class Product
{
    public long Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? Delt { get; set; }
}
