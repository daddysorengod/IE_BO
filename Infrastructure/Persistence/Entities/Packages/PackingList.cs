namespace Infrastructure.Persistence.Entities.Packages;

internal class PackingList
{
    public long Id { get; set; }
    public string PackingListNo { get; set; } = string.Empty;
    public long? ContractId { get; set; }
    public string? PackageName { get; set; }
    public decimal? ProductQuantity { get; set; }
    public string? ContainerNo { get; set; }
    public decimal? TotalCtns { get; set; }
    public decimal? TotalCbm { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? Note { get; set; }
    public string? Delt { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
