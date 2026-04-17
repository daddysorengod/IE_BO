namespace BussinessLayer.Products.Models;

public class GetProductByIdResponse
{
    public long Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public List<ProductImageResponseItem> ListImage { get; set; } = new();
}
