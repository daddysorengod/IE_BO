namespace BussinessLayer.Products.Models;

public class CreateProductRequest
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public List<ProductImageUpsertRequest> Images { get; set; } = new();
}