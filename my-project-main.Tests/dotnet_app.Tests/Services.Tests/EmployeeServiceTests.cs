using dotnet_app.DTOs.Requests;
using dotnet_app.Models.Entities;
using dotnet_app.Services.DTOs.Requests;
using dotnet_app.Services.Implementations;
using my_project_main.Tests.dotnet_app.Tests.Support;
using Xunit.Abstractions;

namespace my_project_main.Tests.dotnet_app.Tests.Services.Tests;

public class EmployeeServiceTests
{
    private readonly ITestOutputHelper _output;

    public EmployeeServiceTests(ITestOutputHelper output)
    {
        _output = output;
    }

    private static DepartmentDto Dept(Guid id, string name = "IT") =>
        new() { Id = id, Name = name };

    private static Task<Employee> CreateEmployeeAsync(
        EmployeeServiceImp service,
        Guid departmentId,
        string email = "user@example.com",
        string first = "Jane",
        string last = "Doe") =>
        service.CreateEmployee(new EmployeeCreate(
            first,
            last,
            email,
            null,
            DateTime.UtcNow.Date,
            "Developer",
            departmentId));

    [Fact(DisplayName = "[Employé — Service] Ajouter un employé (département valide)")]
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
        _output.WriteLine(
            $"→ Employé créé : Id={created.Id}, {created.FirstName} {created.LastName}, Email={created.Email}, DeptId={created.DepartmentId}, Token=(présent)");
    }

    [Fact(DisplayName = "[Employé — Service] Ajouter un employé — département invalide → exception")]
    public async Task CreateEmployee_Throws_WhenDepartmentDoesNotExist()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));
        var wrongDept = Guid.NewGuid();

        var ex = await Assert.ThrowsAsync<Exception>(() =>
            CreateEmployeeAsync(service, wrongDept, "x@y.com"));

        Assert.Contains("département", ex.Message, StringComparison.OrdinalIgnoreCase);
        _output.WriteLine($"→ Exception attendue : {ex.Message}");
    }

    [Fact(DisplayName = "[Employé — Service] Récupérer un employé par id")]
    public async Task GetEmployeeById_ReturnsEmployee_WhenExists()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));
        var created = await CreateEmployeeAsync(service, deptId);

        var found = await service.GetEmployeeById(created.Id);

        Assert.Equal(created.Id, found.Id);
        Assert.Equal(created.Email, found.Email);
        _output.WriteLine($"→ Employé lu : Id={found.Id}, Email={found.Email}, Poste={found.Position}");
    }

    [Fact(DisplayName = "[Employé — Service] Récupérer un employé inexistant → exception")]
    public async Task GetEmployeeById_Missing_Throws()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(
            new DepartmentDto { Id = deptId, Name = "HR" });

        var ex = await Assert.ThrowsAsync<Exception>(() => service.GetEmployeeById(Guid.NewGuid()));
        _output.WriteLine($"→ Exception attendue : {ex.Message}");
    }

    [Fact(DisplayName = "[Employé — Service] Lister tous les employés")]
    public async Task GetAllEmployees_ReturnsEmpty_ThenAllCreated()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));

        Assert.Empty(await service.GetAllEmployees());
        _output.WriteLine("→ Aucun employé au départ (liste vide).");

        await CreateEmployeeAsync(service, deptId, "a@b.com", "A", "One");
        await CreateEmployeeAsync(service, deptId, "b@b.com", "B", "Two");

        var all = await service.GetAllEmployees();
        Assert.Equal(2, all.Count);
        foreach (var e in all)
            _output.WriteLine($"→ Liste : Id={e.Id}, {e.FirstName} {e.LastName}, {e.Email}");
    }

    [Fact(DisplayName = "[Employé — Service] Modifier un employé inexistant → exception")]
    public async Task UpdateEmployee_Missing_Throws()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));

        var ex = await Assert.ThrowsAsync<Exception>(() => service.UpdateEmployee(
            Guid.NewGuid(),
            new EmployeeUpdate("A", "B", "a@b.com", null, DateTime.UtcNow, "P")));
        _output.WriteLine($"→ Exception attendue : {ex.Message}");
    }

    [Fact(DisplayName = "[Employé — Service] Supprimer un employé inexistant → exception")]
    public async Task DeleteEmployee_Missing_Throws()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));

        var ex = await Assert.ThrowsAsync<Exception>(() => service.DeleteEmployee(Guid.NewGuid()));
        _output.WriteLine($"→ Exception attendue : {ex.Message}");
    }

    [Fact(DisplayName = "[Employé — Service] Supprimer un employé")]
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
        _output.WriteLine($"→ Employé Id={created.Id} supprimé, lecture suivante en erreur (attendu).");
    }

    [Fact(DisplayName = "[Employé — Service] Modifier un employé")]
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
        _output.WriteLine(
            $"→ Employé mis à jour : Id={updated.Id}, {updated.FirstName} {updated.LastName}, {updated.Email}, Poste={updated.Position}");
    }

    [Fact(DisplayName = "[Employé — Service] Récupérer un employé par jeton de création")]
    public async Task GetEmployeeByTokenAsync_ReturnsEmployee_WhenTokenValid()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));
        var created = await CreateEmployeeAsync(service, deptId);

        var byToken = await service.GetEmployeeByTokenAsync(created.AccountCreationToken);

        Assert.Equal(created.Id, byToken.Id);
        _output.WriteLine($"→ Employé résolu par jeton : Id={byToken.Id}, Email={byToken.Email}");
    }

    [Fact(DisplayName = "[Employé — Service] Jeton de création invalide → exception")]
    public async Task GetEmployeeByTokenAsync_Throws_WhenTokenInvalid()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));

        var ex = await Assert.ThrowsAsync<Exception>(() => service.GetEmployeeByTokenAsync("not-a-real-token"));
        _output.WriteLine($"→ Exception attendue : {ex.Message}");
    }

    [Fact(DisplayName = "[Employé — Service] Vérifier un employé (active le compte)")]
    public async Task VerifyEmployeeAsync_SetsIsVerified_AndClearsToken()
    {
        var deptId = Guid.NewGuid();
        var (_, service) = EmployeeServiceTestFixture.CreateEmployeeService(Dept(deptId));
        var created = await CreateEmployeeAsync(service, deptId);
        var token = created.AccountCreationToken;

        await service.VerifyEmployeeAsync(created.Id);

        var updated = await service.GetEmployeeById(created.Id);
        Assert.True(updated.IsVerified);
        Assert.Null(updated.AccountCreationToken);
        await Assert.ThrowsAsync<Exception>(() => service.GetEmployeeByTokenAsync(token));
        _output.WriteLine($"→ Employé Id={updated.Id} vérifié : IsVerified=true, jeton effacé.");
    }
}
