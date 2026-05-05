using Domain.Partners;

namespace Application.Partners.Repositories;

public class PartnerSearchResult
{
    public IReadOnlyList<Partner> Items { get; set; } = Array.Empty<Partner>();
    public int Total { get; set; }
}
