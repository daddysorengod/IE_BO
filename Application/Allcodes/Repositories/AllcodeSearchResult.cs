using Domain.Allcodes;

namespace Application.Allcodes.Repositories;

public class AllcodeSearchResult
{
    public IReadOnlyList<Allcode> Items { get; set; } = Array.Empty<Allcode>();
    public int Total { get; set; }
}
