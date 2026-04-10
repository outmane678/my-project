using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services.Implementations;

namespace my_project_main.Tests.WebApplication1.Tests.Services.Tests;

public class DepartmentServiceTests
{
    private static DepartmentServiceImp CreateSut()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        return new DepartmentServiceImp(context);
    }

    [Fact]
    public void CreateDepartment_PersistsAndReturnsDepartment()
    {
        var service = CreateSut();
        var created = service.CreateDepartment(new DepartmentCreate { Name = "RH" });

        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal("RH", created.Name);
    }

    [Fact]
    public void FindDepartmentById_Missing_ReturnsNull()
    {
        var service = CreateSut();

        var found = service.FindDepartmentById(Guid.NewGuid());

        Assert.Null(found);
    }

    [Fact]
    public void UpadateDepartment_Missing_Throws()
    {
        var service = CreateSut();
        var missingId = Guid.NewGuid();

        var ex = Assert.Throws<Exception>(() =>
            service.UpadateDepartment(missingId, new DepartmentUpdate { Name = "X" }));

        Assert.Contains(missingId.ToString(), ex.Message);
    }

    [Fact]
    public void DeleteDepartment_Existing_RemovesFromDatabase()
    {
        var service = CreateSut();
        var created = service.CreateDepartment(new DepartmentCreate { Name = "Temp" });

        service.DeleteDepartment(created.Id);

        Assert.Null(service.FindDepartmentById(created.Id));
    }
}
