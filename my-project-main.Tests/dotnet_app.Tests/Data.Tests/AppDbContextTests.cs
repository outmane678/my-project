using dotnet_app.Data;
using dotnet_app.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace my_project_main.Tests.dotnet_app.Tests.Data.Tests;

public class AppDbContextTests
{
    [Fact(DisplayName = "[Employé — Données] EF Core en mémoire : enregistrer un employé")]
    public void SaveChanges_PersistsEmployeeInMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        var deptId = Guid.NewGuid();
        context.Employees.Add(new Employee
        {
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            HireDate = DateTime.UtcNow,
            Position = "QA",
            AccountCreationToken = "token",
            DepartmentId = deptId
        });
        context.SaveChanges();

        Assert.Single(context.Employees);
    }
}
