using Domain.Contracts;

namespace Application.Contracts.Repositories;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(long contractId, CancellationToken cancellationToken = default);
    Task<Contract?> GetByContractNoAsync(string contractNo, CancellationToken cancellationToken = default);
    Task<long> InsertAsync(Contract contract, CancellationToken cancellationToken = default);
    Task<long> InsertExportAggregateAsync(Contract contract, IReadOnlyCollection<ContractAttachment> attachments, IReadOnlyCollection<ContractPackage> packages, CancellationToken cancellationToken = default);
    Task<long> InsertImportAggregateAsync(Contract contract, IReadOnlyCollection<ContractAttachment> attachments, IReadOnlyCollection<ContractItem> items, CancellationToken cancellationToken = default);
    Task<long> InsertAttachmentAsync(ContractAttachment attachment, CancellationToken cancellationToken = default);
    Task<ContractDetailData?> GetDetailAsync(long contractId, CancellationToken cancellationToken = default);
    Task<ContractSearchResult> SearchAsync(ContractSearchCondition condition, CancellationToken cancellationToken = default);
}
