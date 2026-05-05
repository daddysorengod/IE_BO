using Domain.Common;

namespace Domain.Packages;

public class Package : BaseEntity
{
    public long Id { get; set; }
    public string PackageCode { get; set; } = string.Empty;
    public string PackageName { get; set; } = string.Empty;
    public decimal ProductQuantity { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? Note { get; set; }
    public string? Delt { get; set; }
}
