using Application.Allcodes.Models;
using Application.Allcodes.Repositories;
using Domain.Allcodes;
using Microsoft.Extensions.Logging;

namespace Application.Allcodes;

public class AllcodeService : IAllcodeService
{
    private readonly IAllcodeRepository _allcodeRepository;
    private readonly ILogger<AllcodeService> _logger;

    public AllcodeService(IAllcodeRepository allcodeRepository, ILogger<AllcodeService> logger)
    {
        _allcodeRepository = allcodeRepository;
        _logger = logger;
    }

    public async Task<SearchAllcodeResponse> SearchAsync(SearchAllcodeRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateSearch(request);

            var result = await _allcodeRepository.SearchAsync(new AllcodeSearchCondition
            {
                CdName = request.CdName.Trim().ToUpperInvariant(),
                CdValue = TrimToNull(request.CdValue),
                Display = "Y"
            }, cancellationToken);

            return new SearchAllcodeResponse
            {
                Items = result.Items.Select(Map).ToList(),
                Total = result.Total
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Search allcode failed for cdName: {CdName}", request.CdName);
            throw;
        }
    }

    private static AllcodeResponse Map(Allcode allcode)
    {
        return new AllcodeResponse
        {
            Id = allcode.Id,
            CdName = allcode.CdName,
            CdValue = allcode.CdValue,
            TextVn = allcode.TextVn,
            TextEn = allcode.TextEn,
            Display = allcode.Display
        };
    }

    private static void ValidateSearch(SearchAllcodeRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CdName))
        {
            throw new ArgumentException("CdName is required.");
        }
    }

    private static string? TrimToNull(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }
}
