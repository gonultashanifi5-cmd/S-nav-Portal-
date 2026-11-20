using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnlineSinavPortali.Repositories;

namespace OnlineSinavPortali.Models;

internal static class AdminSetup
{
    public static async Task EnsureCreatedAsync(IServiceProvider services, IConfiguration configuration)
    {
        var userName = configuration["AdminSetup:UserName"]?.Trim();
        var password = configuration["AdminSetup:Password"];
        if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
        {
            return;
        }

        if (string.IsNullOrEmpty(userName) || userName.Length > 60 ||
            string.IsNullOrWhiteSpace(password) || password.Length is < 12 or > 128)
        {
            throw new InvalidOperationException(
                "AdminSetup için en fazla 60 karakterli kullanıcı adı ve 12–128 karakterli bir parola girin.");
        }

        using var scope = services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<GenericRepository<User>>();
        if (await users.Query().AnyAsync(user => user.Role == User.AdminRole))
        {
            return;
        }

        var admin = new User { UserName = userName };
        var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
        admin.PasswordHash = hasher.HashPassword(admin, password);
        await users.AddAsync(admin);
    }
}
