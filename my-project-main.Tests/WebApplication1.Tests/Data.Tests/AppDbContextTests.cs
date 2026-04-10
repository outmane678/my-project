using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace my_project_main.Tests.WebApplication1.Tests.Data.Tests;

public class AppDbContextTests
{
    [Fact(DisplayName = "[Département — Données] EF Core en mémoire : enregistrer un département")]
    public void SaveChanges_PersistsDepartmentInMemory()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);
        context.Departments.Add(new Department { Name = "Finance" });
        context.SaveChanges();

        Assert.Single(context.Departments);
    }
}
