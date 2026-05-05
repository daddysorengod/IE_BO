using Domain.Packages;

namespace Application.Packages.Repositories;

public class PackageDetailData
{
    public Package Package { get; set; } = new();
    public IReadOnlyList<PackageItemData> Items { get; set; } = Array.Empty<PackageItemData>();
}

public class PackageItemData : PackageItem
{
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
}
