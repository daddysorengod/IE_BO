using Application.Contracts.Models;

namespace Application.Contracts;

public interface IContractService
{
    Task<CreateContractResponse> CreateAsync(CreateContractRequest request, long currentUserId, CancellationToken cancellationToken = default);
    Task<CreateContractResponse> CreateImportAsync(CreateImportContractRequest request, long currentUserId, CancellationToken cancellationToken = default);
    Task<ContractDetailResponse?> GetDetailAsync(long contractId, CancellationToken cancellationToken = default);
    Task<AddContractAttachmentResponse> AddAttachmentAsync(long contractId, CreateContractAttachmentRequest request, long currentUserId, CancellationToken cancellationToken = default);
    Task<SearchContractResponse> SearchAsync(SearchContractRequest request, CancellationToken cancellationToken = default);
}
