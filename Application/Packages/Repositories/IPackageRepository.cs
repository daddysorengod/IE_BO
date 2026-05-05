using Domain.Packages;

namespace Application.Packages.Repositories;

public interface IPackageRepository
{
    Task<Package?> GetByIdAsync(long packageId, CancellationToken cancellationToken = default);
    Task<Package?> GetByCodeAsync(string packageCode, CancellationToken cancellationToken = default);
    Task<long> InsertAsync(Package package, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Package package, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long packageId, long currentUserId, DateTime updatedDate, CancellationToken cancellationToken = default);
    Task<PackageSearchResult> SearchAsync(PackageSearchCondition condition, CancellationToken cancellationToken = default);
}
