using Domain.Packages;

namespace Application.Packages.Repositories;

public class PackageSearchResult
{
    public IReadOnlyList<Package> Items { get; set; } = Array.Empty<Package>();
    public int Total { get; set; }
}
