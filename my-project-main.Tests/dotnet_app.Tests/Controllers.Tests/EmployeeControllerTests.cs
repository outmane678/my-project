using System.Collections.Generic;
using System.Linq;
using dotnet_app.Controllers;
using dotnet_app.DTOs.Requests;
using dotnet_app.Models.Entities;
using dotnet_app.Services.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using my_project_main.Tests.dotnet_app.Tests.Support;

namespace my_project_main.Tests.dotnet_app.Tests.Controllers.Tests;

public class EmployeeControllerTests
{
    private static (Guid DeptId, EmployeeController Controller) CreateController()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "IT" });
        return (deptId, new EmployeeController(service));
    }

    [Fact(DisplayName = "[Employé — API] Ajouter un employé (200 OK)")]
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

    [Fact(DisplayName = "[Employé — API] Ajouter un employé — département invalide (400)")]
    public async Task CreateEmployee_ReturnsBadRequest_WhenDepartmentInvalid()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "IT" });
        var controller = new EmployeeController(service);

        var result = await controller.CreateEmployee(new EmployeeCreate(
            "A", "B", "a@b.com", null, DateTime.UtcNow, "P", Guid.NewGuid()));

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.NotNull(bad.Value);
    }

    [Fact(DisplayName = "[Employé — API] Récupérer un employé par id (200 OK)")]
    public async Task GetEmployeeById_ReturnsOk_WhenExists()
    {
        var (deptId, controller) = CreateController();
        var created = await controller.CreateEmployee(new EmployeeCreate(
            "John", "Smith", "john@example.com", null, DateTime.UtcNow.Date, "Dev", deptId));
        var okCreate = Assert.IsType<OkObjectResult>(created);
        var employee = Assert.IsType<Employee>(okCreate.Value);

        var result = await controller.GetEmployeeById(employee.Id);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<Employee>(ok.Value);
        Assert.Equal("John", payload.FirstName);
    }

    [Fact(DisplayName = "[Employé — API] Lister tous les employés (200 OK)")]
    public async Task GetAllEmployees_ReturnsOk_WithCreatedEmployees()
    {
        var (deptId, controller) = CreateController();
        await controller.CreateEmployee(new EmployeeCreate(
            "A", "One", "a1@b.com", null, DateTime.UtcNow, "P", deptId));
        await controller.CreateEmployee(new EmployeeCreate(
            "B", "Two", "b2@b.com", null, DateTime.UtcNow, "P", deptId));

        var result = await controller.GetAllEmployees();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<Employee>>(ok.Value);
        Assert.Equal(2, list!.Count());
    }

    [Fact(DisplayName = "[Employé — API] Modifier un employé (200 OK)")]
    public async Task UpdateEmployee_ReturnsOk_WhenExists()
    {
        var (deptId, controller) = CreateController();
        var created = await controller.CreateEmployee(new EmployeeCreate(
            "Old", "Name", "old@b.com", null, DateTime.UtcNow, "Junior", deptId));
        var emp = Assert.IsType<Employee>(Assert.IsType<OkObjectResult>(created).Value);

        var result = await controller.UpdateEmployee(
            emp.Id,
            new EmployeeUpdate("New", "Name", "new@b.com", null, DateTime.UtcNow.Date, "Senior"));

        var ok = Assert.IsType<OkObjectResult>(result);
        var updated = Assert.IsType<Employee>(ok.Value);
        Assert.Equal("New", updated.FirstName);
        Assert.Equal("Senior", updated.Position);
    }

    [Fact(DisplayName = "[Employé — API] Modifier un employé inexistant (404)")]
    public async Task UpdateEmployee_ReturnsNotFound_WhenMissing()
    {
        var (_, controller) = CreateController();

        var result = await controller.UpdateEmployee(
            Guid.NewGuid(),
            new EmployeeUpdate("A", "B", "a@b.com", null, DateTime.UtcNow, "P"));

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact(DisplayName = "[Employé — API] Supprimer un employé (204 No Content)")]
    public async Task DeleteEmployee_ReturnsNoContent_WhenExists()
    {
        var (deptId, controller) = CreateController();
        var created = await controller.CreateEmployee(new EmployeeCreate(
            "Del", "Me", "del@b.com", null, DateTime.UtcNow, "P", deptId));
        var emp = Assert.IsType<Employee>(Assert.IsType<OkObjectResult>(created).Value);

        var result = await controller.DeleteEmployee(emp.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact(DisplayName = "[Employé — API] Supprimer un employé inexistant (404)")]
    public async Task DeleteEmployee_ReturnsNotFound_WhenMissing()
    {
        var (_, controller) = CreateController();

        var result = await controller.DeleteEmployee(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact(DisplayName = "[Employé — API] Récupérer un employé par jeton (200 OK)")]
    public async Task GetEmployeeByToken_ReturnsOk_WhenTokenValid()
    {
        var (deptId, controller) = CreateController();
        var created = await controller.CreateEmployee(new EmployeeCreate(
            "Tok", "User", "tok@b.com", null, DateTime.UtcNow, "P", deptId));
        var emp = Assert.IsType<Employee>(Assert.IsType<OkObjectResult>(created).Value);

        var result = await controller.GetEmployeeByToken(emp.AccountCreationToken);

        var ok = Assert.IsType<OkObjectResult>(result);
        var payload = Assert.IsType<Employee>(ok.Value);
        Assert.Equal(emp.Id, payload.Id);
    }

    [Fact(DisplayName = "[Employé — API] Jeton invalide (404)")]
    public async Task GetEmployeeByToken_ReturnsNotFound_WhenInvalid()
    {
        var (_, controller) = CreateController();

        var result = await controller.GetEmployeeByToken("invalid-token");

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact(DisplayName = "[Employé — API] Vérifier un employé (204 No Content)")]
    public async Task VerifyEmployee_ReturnsNoContent_WhenExists()
    {
        var (deptId, controller) = CreateController();
        var created = await controller.CreateEmployee(new EmployeeCreate(
            "Ver", "Ify", "v@b.com", null, DateTime.UtcNow, "P", deptId));
        var emp = Assert.IsType<Employee>(Assert.IsType<OkObjectResult>(created).Value);

        var result = await controller.VerifyEmployee(emp.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact(DisplayName = "[Employé — API] Récupérer un employé inexistant (404)")]
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
