namespace Application.Products.Models;

public class GetProductByIdResponse
{
    public long Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public string? Material { get; set; }
    public int? ProductionYear { get; set; }
    public string? Note { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public List<ProductImageResponseItem> ListImage { get; set; } = new();
}
