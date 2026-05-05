using Application.Allcodes.Models;

namespace Application.Allcodes;

public interface IAllcodeService
{
    Task<SearchAllcodeResponse> SearchAsync(SearchAllcodeRequest request, CancellationToken cancellationToken = default);
}
