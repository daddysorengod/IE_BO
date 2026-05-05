namespace Application.Allcodes.Repositories;

public interface IAllcodeRepository
{
    Task<AllcodeSearchResult> SearchAsync(AllcodeSearchCondition condition, CancellationToken cancellationToken = default);
}
