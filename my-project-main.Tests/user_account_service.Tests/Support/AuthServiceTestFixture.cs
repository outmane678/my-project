using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using user_account_service.Config;
using user_account_service.Data;
using user_account_service.DTOs;
using user_account_service.Services.Implementations;

namespace my_project_main.Tests.user_account_service.Tests.Support;

internal static class AuthServiceTestFixture
{
    internal const string JwtSecretKey = "0123456789abcdef0123456789abcdef01234567";

    public static (AppDbContext Context, AuthService Auth, PasswordService Password, JwtTokenService Jwt)
        CreateAuthService(EmployeeDTO? employeeForSignupToken, string? databaseName = null)
    {
        databaseName ??= Guid.NewGuid().ToString();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;
        var context = new AppDbContext(options);
        var passwordService = new PasswordService();
        IConfiguration configuration = new TestConfiguration(new Dictionary<string, string?>
        {
            ["Jwt:Secret"] = JwtSecretKey
        });
        var jwtService = new JwtTokenService(configuration);
        var api = new StubEmployeeApi(employeeForSignupToken);
        var auth = new AuthService(context, passwordService, jwtService, api);
        return (context, auth, passwordService, jwtService);
    }
}
