using Application.Partners.Models;

namespace Application.Partners;

public interface IPartnerService
{
    Task<PartnerResponse?> GetByIdAsync(long partnerId, CancellationToken cancellationToken = default);
    Task<CreatePartnerResponse> CreateAsync(CreatePartnerRequest request, long currentUserId, CancellationToken cancellationToken = default);
    Task<UpdatePartnerResponse> UpdateAsync(UpdatePartnerRequest request, long currentUserId, CancellationToken cancellationToken = default);
    Task<SearchPartnerResponse> SearchAsync(SearchPartnerRequest request, CancellationToken cancellationToken = default);
}
