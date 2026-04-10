using System.Collections.Generic;
using System.Linq;
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

    [Fact(DisplayName = "[Département — API] Ajouter un département (POST → 200 OK)")]
    public void CreateDepartment_ReturnsOk()
    {
        var controller = CreateController();

        var result = controller.CreateDepartment(new DepartmentCreate { Name = "IT" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var payload = Assert.IsType<Department>(ok.Value);
        Assert.Equal("IT", payload.Name);
    }

    [Fact(DisplayName = "[Département — API] Récupérer un département par id (GET → 200 OK)")]
    public void GetDepartmentById_ReturnsOk_WhenExists()
    {
        var controller = CreateController();
        var created = Assert.IsType<Department>(
            Assert.IsType<OkObjectResult>(
                controller.CreateDepartment(new DepartmentCreate { Name = "Finance" }).Result).Value);

        var result = controller.GetDepartmentById(created.Id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("Finance", Assert.IsType<Department>(ok.Value).Name);
    }

    [Fact(DisplayName = "[Département — API] Récupérer un département inexistant (GET → 404)")]
    public void GetDepartmentById_ReturnsNotFound_WhenMissing()
    {
        var controller = CreateController();

        var result = controller.GetDepartmentById(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact(DisplayName = "[Département — API] Lister tous les départements (GET → 200 OK)")]
    public void GetAllDepartments_ReturnsOk_WithList()
    {
        var controller = CreateController();
        controller.CreateDepartment(new DepartmentCreate { Name = "D1" });
        controller.CreateDepartment(new DepartmentCreate { Name = "D2" });

        var result = controller.GetAllDepartments();

        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsAssignableFrom<IEnumerable<Department>>(ok.Value);
        Assert.Equal(2, list!.Count());
    }

    [Fact(DisplayName = "[Département — API] Modifier un département (PUT → 200 OK)")]
    public void UpdateDepartment_ReturnsOk_WhenExists()
    {
        var controller = CreateController();
        var dept = Assert.IsType<Department>(
            Assert.IsType<OkObjectResult>(
                controller.CreateDepartment(new DepartmentCreate { Name = "Old" }).Result).Value);

        var result = controller.UpdateDepartment(dept.Id, new DepartmentUpdate { Name = "New" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal("New", Assert.IsType<Department>(ok.Value).Name);
    }

    [Fact(DisplayName = "[Département — API] Modifier un département inexistant (PUT → 404)")]
    public void UpdateDepartment_ReturnsNotFound_WhenMissing()
    {
        var controller = CreateController();

        var result = controller.UpdateDepartment(
            Guid.NewGuid(),
            new DepartmentUpdate { Name = "X" });

        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact(DisplayName = "[Département — API] Supprimer un département (DELETE → 204)")]
    public void DeleteDepartment_ReturnsNoContent_WhenExists()
    {
        var controller = CreateController();
        var dept = Assert.IsType<Department>(
            Assert.IsType<OkObjectResult>(
                controller.CreateDepartment(new DepartmentCreate { Name = "ToDelete" }).Result).Value);

        var result = controller.DeleteDepartment(dept.Id);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact(DisplayName = "[Département — API] Supprimer un département inexistant (DELETE → 404)")]
    public void DeleteDepartment_ReturnsNotFound_WhenMissing()
    {
        var controller = CreateController();

        var result = controller.DeleteDepartment(Guid.NewGuid());

        Assert.IsType<NotFoundObjectResult>(result);
    }
}
