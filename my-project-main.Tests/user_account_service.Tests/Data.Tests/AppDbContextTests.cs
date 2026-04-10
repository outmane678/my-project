using Microsoft.EntityFrameworkCore;
using user_account_service.Data;
using user_account_service.Models.Entities;

namespace my_project_main.Tests.user_account_service.Tests.Data.Tests;

public class AppDbContextTests
{
    [Fact(DisplayName = "[Compte — Données] EF Core en mémoire : enregistrer un utilisateur")]
    public void SaveChanges_PersistsUserAccountInMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        context.UserAccounts.Add(new UserAccount
        {
            Username = "u1",
            Password = "hash",
            Role = "USER",
            EmployeeId = Guid.NewGuid()
        });
        context.SaveChanges();

        Assert.Single(context.UserAccounts);
    }
}
