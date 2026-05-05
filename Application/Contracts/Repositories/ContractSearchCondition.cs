namespace Application.Contracts.Repositories;

public class ContractSearchCondition
{
    public string? Keyword { get; set; }
    public string? ContractType { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
}
