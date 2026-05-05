using Application.Packages.Models;

namespace Application.Packages;

public interface IPackageService
{
    Task<PackageResponse?> GetByIdAsync(long packageId, CancellationToken cancellationToken = default);
    Task<SearchPackageResponse> SearchAsync(SearchPackageRequest request, CancellationToken cancellationToken = default);
    Task<CreatePackageResponse> CreateAsync(CreatePackageRequest request, long currentUserId, CancellationToken cancellationToken = default);
    Task<UpdatePackageResponse> UpdateAsync(long packageId, UpdatePackageRequest request, long currentUserId, CancellationToken cancellationToken = default);
    Task<DeletePackageResponse> DeleteAsync(long packageId, long currentUserId, CancellationToken cancellationToken = default);
}
