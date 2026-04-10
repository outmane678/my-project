using dotnet_app.Controllers;
using dotnet_app.DTOs.Requests;
using dotnet_app.Models.Entities;
using dotnet_app.Services.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using my_project_main.Tests.dotnet_app.Tests.Support;

namespace my_project_main.Tests.dotnet_app.Tests.Controllers.Tests;

public class EmployeeControllerTests
{
    [Fact]
    public async Task CreateEmployee_ReturnsOkObjectResult_WhenDepartmentExists()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "IT" });
        var controller = new EmployeeController(service);

        var result = await controller.CreateEmployee(new EmployeeCreate(
            "Jane",
            "Doe",
            "jane@example.com",
            null,
            DateTime.UtcNow.Date,
            "Developer",
            deptId));

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<Employee>(ok.Value);
        Assert.Equal("Jane", payload.FirstName);
    }

    [Fact]
    public async Task GetEmployeeById_Missing_ReturnsNotFound()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "IT" });
        var controller = new EmployeeController(service);

        var result = await controller.GetEmployeeById(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
