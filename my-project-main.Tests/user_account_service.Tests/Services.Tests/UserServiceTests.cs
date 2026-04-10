using dotnet_app.Services;
using Microsoft.EntityFrameworkCore;
using user_account_service.Data;
using user_account_service.Models.Entities;

namespace my_project_main.Tests.user_account_service.Tests.Services.Tests;

public class UserServiceTests
{
    [Fact(DisplayName = "[Compte — Service] Charger un utilisateur par nom (existe)")]
    public async Task LoadUserByUsername_ReturnsUser_WhenExists()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        context.UserAccounts.Add(new UserAccount
        {
            Username = "findme",
            Password = "hash",
            Role = "USER",
            EmployeeId = Guid.NewGuid()
        });
        await context.SaveChangesAsync();

        var service = new UserService(context);
        var user = await service.LoadUserByUsername("findme");

        Assert.NotNull(user);
        Assert.Equal("findme", user!.Username);
    }

    [Fact(DisplayName = "[Compte — Service] Charger un utilisateur par nom (inexistant → null)")]
    public async Task LoadUserByUsername_ReturnsNull_WhenMissing()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AppDbContext(options);
        var service = new UserService(context);

        var user = await service.LoadUserByUsername("nope");

        Assert.Null(user);
    }
}
