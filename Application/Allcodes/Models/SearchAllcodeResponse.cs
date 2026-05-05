namespace Application.Allcodes.Models;

public class SearchAllcodeResponse
{
    public List<AllcodeResponse> Items { get; set; } = new();
    public int Total { get; set; }
}
