using dotnet_app.DTOs.Requests;
using dotnet_app.Services.DTOs.Requests;
using my_project_main.Tests.dotnet_app.Tests.Support;

namespace my_project_main.Tests.dotnet_app.Tests.Services.Tests;

public class EmployeeServiceTests
{
    [Fact]
    public async Task CreateEmployee_PersistsAndReturnsEmployee_WhenDepartmentExists()
    {
        var deptId = Guid.NewGuid();
        var department = new DepartmentDto { Id = deptId, Name = "IT" };
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(department);

        var created = await service.CreateEmployee(new EmployeeCreate(
            "Jane",
            "Doe",
            "jane@example.com",
            null,
            DateTime.UtcNow.Date,
            "Developer",
            deptId));

        Assert.Equal("Jane", created.FirstName);
        Assert.Equal(deptId, created.DepartmentId);
        Assert.False(created.IsVerified);
        Assert.False(string.IsNullOrEmpty(created.AccountCreationToken));
    }

    [Fact]
    public async Task GetEmployeeById_Missing_Throws()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "HR" });

        await Assert.ThrowsAsync<Exception>(() => service.GetEmployeeById(Guid.NewGuid()));
    }

    [Fact]
    public async Task DeleteEmployee_Existing_RemovesFromDatabase()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "Finance" });
        var created = await service.CreateEmployee(new EmployeeCreate(
            "A",
            "B",
            "a@b.com",
            null,
            DateTime.UtcNow,
            "Role",
            deptId));

        await service.DeleteEmployee(created.Id);

        await Assert.ThrowsAsync<Exception>(() => service.GetEmployeeById(created.Id));
    }

    [Fact]
    public async Task UpdateEmployee_Existing_UpdatesFields()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "Ops" });
        var created = await service.CreateEmployee(new EmployeeCreate(
            "Old",
            "Name",
            "old@b.com",
            null,
            DateTime.UtcNow,
            "Junior",
            deptId));

        var updated = await service.UpdateEmployee(
            created.Id,
            new EmployeeUpdate(
                "New",
                "Surname",
                "new@b.com",
                "0600000000",
                DateTime.UtcNow.Date,
                "Lead"));

        Assert.Equal("New", updated.FirstName);
        Assert.Equal("Lead", updated.Position);
        Assert.Equal("new@b.com", updated.Email);
    }
}
