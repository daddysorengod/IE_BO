namespace Application.Security;

public sealed class SuperUserOptions
{
    public string SupperUser { get; set; } = string.Empty;
    public string SupperPassword { get; set; } = string.Empty;
    public long UserId { get; set; } = 999999;
    public string FullName { get; set; } = "Supper Admin";
    public string Role { get; set; } = "SuperAdmin";
    public string? ImageUrl { get; set; }

    public bool IsEnabled()
    {
        return !string.IsNullOrWhiteSpace(SupperUser)
            && !string.IsNullOrWhiteSpace(SupperPassword);
    }

    public void Validate()
    {
        if (!IsEnabled())
        {
            return;
        }

        if (UserId <= 0)
        {
            throw new InvalidOperationException("SuperAdmin:userid must be greater than 0.");
        }

        if (string.IsNullOrWhiteSpace(Role))
        {
            throw new InvalidOperationException("SuperAdmin:role is required.");
        }

        if (string.IsNullOrWhiteSpace(FullName))
        {
            throw new InvalidOperationException("SuperAdmin:fullname is required.");
        }
    }
}
