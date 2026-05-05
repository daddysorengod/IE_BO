namespace Infrastructure.Persistence.Entities.Packages;

internal class PackingListItem
{
    public long Id { get; set; }
    public long PackingListId { get; set; }
    public long ProductId { get; set; }
    public decimal TotalUnits { get; set; }
    public decimal? TotalCtns { get; set; }
    public decimal? TotalCbm { get; set; }
    public long? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public long? UpdatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
