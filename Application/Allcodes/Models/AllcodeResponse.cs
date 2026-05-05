namespace Application.Allcodes.Models;

public class AllcodeResponse
{
    public long Id { get; set; }
    public string CdName { get; set; } = string.Empty;
    public string CdValue { get; set; } = string.Empty;
    public string TextVn { get; set; } = string.Empty;
    public string? TextEn { get; set; }
    public string Display { get; set; } = "Y";
}
