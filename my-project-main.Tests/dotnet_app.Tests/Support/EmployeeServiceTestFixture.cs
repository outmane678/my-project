using dotnet_app.Data;
using dotnet_app.DTOs.Requests;
using dotnet_app.Services;
using dotnet_app.Services.Implementations;
using Microsoft.EntityFrameworkCore;

namespace my_project_main.Tests.dotnet_app.Tests.Support;

internal sealed class NoOpEmailSender : IEmailSender
{
    public void SendAccountCreationEmail(string to, string token)
    {
    }
}

internal static class EmployeeServiceTestFixture
{
    /// <summary>
    /// Crée le service employé avec EF InMemory, un stub d'API département et un envoi d'e-mail factice (pas de SMTP).
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
        IEmailSender email = new NoOpEmailSender();
        var service = new EmployeeServiceImp(context, api, email);
        return (context, service);
    }
}
