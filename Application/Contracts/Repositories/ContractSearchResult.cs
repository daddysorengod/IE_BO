using Domain.Contracts;

namespace Application.Contracts.Repositories;

public class ContractSearchResult
{
    public IReadOnlyList<Contract> Items { get; set; } = Array.Empty<Contract>();
    public int Total { get; set; }
}
