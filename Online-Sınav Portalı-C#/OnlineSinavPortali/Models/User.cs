namespace OnlineSinavPortali.Models;

public sealed class User : BaseEntity
{
    public const string AdminRole = "Admin";

    public string UserName { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string Role { get; set; } = AdminRole;
}
