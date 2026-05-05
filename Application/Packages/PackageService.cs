using Application.Packages.Models;
using Application.Packages.Repositories;
using Domain.Packages;
using Microsoft.Extensions.Logging;

namespace Application.Packages;

public class PackageService : IPackageService
{
    private readonly IPackageRepository _packageRepository;
    private readonly ILogger<PackageService> _logger;

    public PackageService(IPackageRepository packageRepository, ILogger<PackageService> logger)
    {
        _packageRepository = packageRepository;
        _logger = logger;
    }

    public async Task<PackageResponse?> GetByIdAsync(long packageId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(packageId);
            var package = await _packageRepository.GetByIdAsync(packageId, cancellationToken);
            return package is null ? null : MapPackage(package);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get package failed for id: {Id}", packageId);
            throw;
        }
    }

    public async Task<SearchPackageResponse> SearchAsync(SearchPackageRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidatePaging(request.Page, request.PageSize);

            var result = await _packageRepository.SearchAsync(new PackageSearchCondition
            {
                Keyword = request.Keyword,
                Offset = (request.Page - 1) * request.PageSize,
                Limit = request.PageSize
            }, cancellationToken);

            var totalPages = result.Total == 0 ? 0 : (int)Math.Ceiling(result.Total / (double)request.PageSize);
            return new SearchPackageResponse
            {
                Items = result.Items.Select(MapPackage).ToList(),
                Total = result.Total,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search package failed.");
            throw;
        }
    }

    public async Task<CreatePackageResponse> CreateAsync(CreatePackageRequest request, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateCreate(request);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var packageCode = request.PackageCode.Trim();
            var existing = await _packageRepository.GetByCodeAsync(packageCode, cancellationToken);
            if (existing is not null)
            {
                throw new InvalidOperationException("PackageCode already exists.");
            }

            var now = DateTime.Now;
            var package = new Package
            {
                PackageCode = packageCode,
                PackageName = request.PackageName.Trim(),
                ProductQuantity = request.ProductQuantity,
                Length = request.Length,
                Width = request.Width,
                Height = request.Height,
                Note = TrimToNull(request.Note),
                CreatedBy = currentUserId,
                CreatedDate = now
            };

            var id = await _packageRepository.InsertAsync(package, cancellationToken);
            return new CreatePackageResponse { Id = id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create package failed for package code: {PackageCode}", request.PackageCode);
            throw;
        }
    }

    public async Task<UpdatePackageResponse> UpdateAsync(long packageId, UpdatePackageRequest request, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(packageId);
            ValidateUpdate(request);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var current = await _packageRepository.GetByIdAsync(packageId, cancellationToken);
            if (current is null)
            {
                return new UpdatePackageResponse { Id = packageId, IsUpdated = false };
            }

            var packageCode = request.PackageCode.Trim();
            var existing = await _packageRepository.GetByCodeAsync(packageCode, cancellationToken);
            if (existing is not null && existing.Id != packageId)
            {
                throw new InvalidOperationException("PackageCode already exists.");
            }

            var package = new Package
            {
                Id = packageId,
                PackageCode = packageCode,
                PackageName = request.PackageName.Trim(),
                ProductQuantity = request.ProductQuantity,
                Length = request.Length,
                Width = request.Width,
                Height = request.Height,
                Note = TrimToNull(request.Note),
                UpdatedBy = currentUserId,
                UpdatedDate = DateTime.Now
            };

            var updated = await _packageRepository.UpdateAsync(package, cancellationToken);
            return new UpdatePackageResponse { Id = packageId, IsUpdated = updated };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update package failed for id: {Id}", packageId);
            throw;
        }
    }

    public async Task<DeletePackageResponse> DeleteAsync(long packageId, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(packageId);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var deleted = await _packageRepository.DeleteAsync(packageId, currentUserId, DateTime.Now, cancellationToken);
            return new DeletePackageResponse { Id = packageId, IsDeleted = deleted };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Delete package failed for id: {Id}", packageId);
            throw;
        }
    }

    private static PackageResponse MapPackage(Package package)
    {
        return new PackageResponse
        {
            Id = package.Id,
            PackageCode = package.PackageCode,
            PackageName = package.PackageName,
            ProductQuantity = package.ProductQuantity,
            Length = package.Length,
            Width = package.Width,
            Height = package.Height,
            Note = package.Note,
            CreatedBy = package.CreatedBy,
            CreatedDate = package.CreatedDate,
            UpdatedBy = package.UpdatedBy,
            UpdatedDate = package.UpdatedDate
        };
    }

    private static void ValidateCreate(CreatePackageRequest request)
    {
        ValidatePackageFields(request.PackageCode, request.PackageName, request.ProductQuantity, request.Length, request.Width, request.Height);
    }

    private static void ValidateUpdate(UpdatePackageRequest request)
    {
        ValidatePackageFields(request.PackageCode, request.PackageName, request.ProductQuantity, request.Length, request.Width, request.Height);
    }

    private static void ValidatePackageFields(string packageCode, string packageName, decimal productQuantity, decimal? length, decimal? width, decimal? height)
    {
        if (string.IsNullOrWhiteSpace(packageCode))
        {
            throw new ArgumentException("PackageCode is required.");
        }

        if (string.IsNullOrWhiteSpace(packageName))
        {
            throw new ArgumentException("PackageName is required.");
        }

        if (productQuantity <= 0)
        {
            throw new ArgumentException("ProductQuantity must be greater than 0.");
        }

        ValidateNonNegative(length, "Length");
        ValidateNonNegative(width, "Width");
        ValidateNonNegative(height, "Height");
    }

    private static void ValidatePaging(int page, int pageSize)
    {
        if (page <= 0)
        {
            throw new ArgumentException("Page must be greater than 0.");
        }

        if (pageSize <= 0 || pageSize > 200)
        {
            throw new ArgumentException("PageSize must be between 1 and 200.");
        }
    }

    private static void ValidateNonNegative(decimal? value, string fieldName)
    {
        if (value is < 0)
        {
            throw new ArgumentException($"{fieldName} must be greater than or equal to 0.");
        }
    }

    private static void ValidateId(long id, string message = "Id must be greater than 0.")
    {
        if (id <= 0)
        {
            throw new ArgumentException(message);
        }
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
