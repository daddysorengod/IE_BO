using Application.Partners.Models;
using Application.Partners.Repositories;
using Domain.Partners;
using Microsoft.Extensions.Logging;

namespace Application.Partners;

public class PartnerService : IPartnerService
{
    private static readonly HashSet<string> AllowedPartnerTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "CUSTOMER",
        "SUPPLIER",
        "SHIPPING"
    };

    private readonly IPartnerRepository _partnerRepository;
    private readonly ILogger<PartnerService> _logger;

    public PartnerService(IPartnerRepository partnerRepository, ILogger<PartnerService> logger)
    {
        _partnerRepository = partnerRepository;
        _logger = logger;
    }

    public async Task<PartnerResponse?> GetByIdAsync(long partnerId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateId(partnerId);
            var partner = await _partnerRepository.GetByIdAsync(partnerId, cancellationToken);
            return partner is null ? null : Map(partner);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get partner failed for id: {Id}", partnerId);
            throw;
        }
    }

    public async Task<CreatePartnerResponse> CreateAsync(CreatePartnerRequest request, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateCreate(request);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var partnerCode = request.PartnerCode.Trim();
            var existing = await _partnerRepository.GetByCodeAsync(partnerCode, cancellationToken);
            if (existing is not null)
            {
                throw new InvalidOperationException("PartnerCode already exists.");
            }

            var partner = new Partner
            {
                PartnerCode = partnerCode,
                PartnerName = request.PartnerName.Trim(),
                PartnerType = request.PartnerType.Trim().ToUpperInvariant(),
                Address = TrimToNull(request.Address),
                Phone = TrimToNull(request.Phone),
                Email = TrimToNull(request.Email),
                IsActive = request.IsActive,
                CreatedBy = currentUserId,
                CreatedDate = DateTime.Now
            };

            var id = await _partnerRepository.InsertAsync(partner, cancellationToken);
            return new CreatePartnerResponse { Id = id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Create partner failed for code: {PartnerCode}", request.PartnerCode);
            throw;
        }
    }

    public async Task<UpdatePartnerResponse> UpdateAsync(UpdatePartnerRequest request, long currentUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateUpdate(request);
            ValidateId(currentUserId, "Current user id must be greater than 0.");

            var existing = await _partnerRepository.GetByIdAsync(request.Id, cancellationToken);
            if (existing is null)
            {
                return new UpdatePartnerResponse { Id = request.Id, IsUpdated = false };
            }

            var partnerCode = request.PartnerCode.Trim();
            var codeOwner = await _partnerRepository.GetByCodeAsync(partnerCode, cancellationToken);
            if (codeOwner is not null && codeOwner.Id != request.Id)
            {
                throw new InvalidOperationException("PartnerCode already exists.");
            }

            existing.PartnerCode = partnerCode;
            existing.PartnerName = request.PartnerName.Trim();
            existing.PartnerType = request.PartnerType.Trim().ToUpperInvariant();
            existing.Address = TrimToNull(request.Address);
            existing.Phone = TrimToNull(request.Phone);
            existing.Email = TrimToNull(request.Email);
            existing.IsActive = request.IsActive;
            existing.UpdatedBy = currentUserId;
            existing.UpdatedDate = DateTime.Now;

            var updated = await _partnerRepository.UpdateAsync(existing, cancellationToken);
            return new UpdatePartnerResponse { Id = request.Id, IsUpdated = updated };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Update partner failed for id: {Id}", request.Id);
            throw;
        }
    }

    public async Task<SearchPartnerResponse> SearchAsync(SearchPartnerRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidatePaging(request.Page, request.PageSize);
            if (!string.IsNullOrWhiteSpace(request.PartnerType))
            {
                ValidatePartnerType(request.PartnerType);
            }

            var result = await _partnerRepository.SearchAsync(new PartnerSearchCondition
            {
                Keyword = request.Keyword,
                PartnerType = TrimToNull(request.PartnerType)?.ToUpperInvariant(),
                IsActive = request.IsActive,
                Offset = (request.Page - 1) * request.PageSize,
                Limit = request.PageSize
            }, cancellationToken);

            var totalPages = result.Total == 0 ? 0 : (int)Math.Ceiling(result.Total / (double)request.PageSize);
            return new SearchPartnerResponse
            {
                Items = result.Items.Select(Map).ToList(),
                Total = result.Total,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalPages = totalPages
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search partner failed.");
            throw;
        }
    }

    private static PartnerResponse Map(Partner partner)
    {
        return new PartnerResponse
        {
            Id = partner.Id,
            PartnerCode = partner.PartnerCode,
            PartnerName = partner.PartnerName,
            PartnerType = partner.PartnerType,
            Address = partner.Address,
            Phone = partner.Phone,
            Email = partner.Email,
            IsActive = partner.IsActive,
            CreatedBy = partner.CreatedBy,
            CreatedDate = partner.CreatedDate,
            UpdatedBy = partner.UpdatedBy,
            UpdatedDate = partner.UpdatedDate
        };
    }

    private static void ValidateCreate(CreatePartnerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.PartnerCode))
        {
            throw new ArgumentException("PartnerCode is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PartnerName))
        {
            throw new ArgumentException("PartnerName is required.");
        }

        ValidatePartnerType(request.PartnerType);
    }

    private static void ValidateUpdate(UpdatePartnerRequest request)
    {
        ValidateId(request.Id);

        if (string.IsNullOrWhiteSpace(request.PartnerCode))
        {
            throw new ArgumentException("PartnerCode is required.");
        }

        if (string.IsNullOrWhiteSpace(request.PartnerName))
        {
            throw new ArgumentException("PartnerName is required.");
        }

        ValidatePartnerType(request.PartnerType);
    }

    private static void ValidatePartnerType(string partnerType)
    {
        if (string.IsNullOrWhiteSpace(partnerType))
        {
            throw new ArgumentException("PartnerType is required.");
        }

        if (!AllowedPartnerTypes.Contains(partnerType.Trim()))
        {
            throw new ArgumentException("PartnerType must be CUSTOMER, SUPPLIER, or SHIPPING.");
        }
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
