using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Controllers;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services.Implementations;

namespace my_project_main.Tests.WebApplication1.Tests.Controllers.Tests;

public class DepartmentsControllerTests
{
    private static DepartmentsController CreateController()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new AppDbContext(options);
        var service = new DepartmentServiceImp(context);
        return new DepartmentsController(service);
    }

    [Fact]
    public void CreateDepartment_ReturnsOkObjectResult()
    {
        var controller = CreateController();

        var result = controller.CreateDepartment(new DepartmentCreate { Name = "IT" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<Department>(ok.Value);
        Assert.Equal("IT", payload.Name);
    }
}
