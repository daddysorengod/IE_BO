using Domain.Partners;

namespace Application.Partners.Repositories;

public interface IPartnerRepository
{
    Task<Partner?> GetByIdAsync(long partnerId, CancellationToken cancellationToken = default);
    Task<Partner?> GetByCodeAsync(string partnerCode, CancellationToken cancellationToken = default);
    Task<long> InsertAsync(Partner partner, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(Partner partner, CancellationToken cancellationToken = default);
    Task<PartnerSearchResult> SearchAsync(PartnerSearchCondition condition, CancellationToken cancellationToken = default);
}
