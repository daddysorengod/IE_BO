namespace Application.Packages.Repositories;

public class PackageSearchCondition
{
    public string? Keyword { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}
