using dotnet_app.Data;
using dotnet_app.DTOs.Requests;
using dotnet_app.Services;
using dotnet_app.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace my_project_main.Tests.dotnet_app.Tests.Support;

internal static class EmployeeServiceTestFixture
{
    /// <summary>
    /// Crée le service employé avec EF InMemory, un stub d'API département et une config email minimale
    /// (l'échec SMTP éventuel est absorbé par <see cref="EmployeeServiceImp.CreateEmployee"/>).
    /// </summary>
    public static (AppDbContext Context, EmployeeServiceImp Service) CreateEmployeeService(
        DepartmentDto department,
        string? databaseName = null)
    {
        databaseName ??= Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        var context = new AppDbContext(options);
        var api = new StubDepartmentApi(department);
        IConfiguration config = new TestConfiguration(new Dictionary<string, string?>
        {
            ["EmailSettings:BackendOrigin"] = "http://localhost",
            ["EmailSettings:Username"] = "test@example.com",
            ["EmailSettings:Password"] = "pwd",
            ["EmailSettings:Host"] = "127.0.0.1",
            ["EmailSettings:Port"] = "1"
        });
        var emailService = new EmailService(config);
        var service = new EmployeeServiceImp(context, api, emailService);
        return (context, service);
    }
}
