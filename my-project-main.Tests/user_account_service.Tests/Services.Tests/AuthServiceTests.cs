using Microsoft.EntityFrameworkCore;
using user_account_service.DTOs;
using user_account_service.DTOs.Requests;
using user_account_service.Models.Entities;
using my_project_main.Tests.user_account_service.Tests.Support;

namespace my_project_main.Tests.user_account_service.Tests.Services.Tests;

public class AuthServiceTests
{
    private static EmployeeDTO SampleEmployee(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        FirstName = "Test",
        LastName = "User",
        Phone = "0600000000",
        Position = "Dev",
        IsVerified = false
    };

    [Fact(DisplayName = "[Auth — Service] Connexion retourne un JWT")]
    public async Task Login_ReturnsJwt_WhenCredentialsValid()
    {
        var (_, auth, _, _) = AuthServiceTestFixture.CreateAuthService(SampleEmployee());
        await auth.Signup(new SignupRequestDto("alice", "Secret123!"), "any-token");
        // Mot de passe en clair pour la requête login (hash stocké en base)
        var jwt = await auth.Login(new LoginRequestDto("alice", "Secret123!"));

        Assert.False(string.IsNullOrWhiteSpace(jwt));
    }

    [Fact(DisplayName = "[Auth — Service] Connexion — utilisateur inconnu → exception")]
    public async Task Login_Throws_WhenUserMissing()
    {
        var (_, auth, _, _) = AuthServiceTestFixture.CreateAuthService(SampleEmployee());

        await Assert.ThrowsAsync<Exception>(() =>
            auth.Login(new LoginRequestDto("nobody", "x")));
    }

    [Fact(DisplayName = "[Auth — Service] Connexion — mot de passe incorrect → exception")]
    public async Task Login_Throws_WhenPasswordInvalid()
    {
        var (_, auth, _, _) = AuthServiceTestFixture.CreateAuthService(SampleEmployee());
        await auth.Signup(new SignupRequestDto("bob", "GoodPass1!"), "tok");

        await Assert.ThrowsAsync<Exception>(() =>
            auth.Login(new LoginRequestDto("bob", "WrongPass!")));
    }

    [Fact(DisplayName = "[Auth — Service] Inscription avec jeton employé valide crée le compte")]
    public async Task Signup_CreatesUser_WhenTokenValid()
    {
        var emp = SampleEmployee();
        var (ctx, auth, _, _) = AuthServiceTestFixture.CreateAuthService(emp);

        await auth.Signup(new SignupRequestDto("carol", "P@ssw0rd!"), "activation-token");

        Assert.True(await ctx.UserAccounts.AnyAsync(u => u.Username == "carol" && u.EmployeeId == emp.Id));
    }

    [Fact(DisplayName = "[Auth — Service] Inscription — nom d’utilisateur déjà pris → exception")]
    public async Task Signup_Throws_WhenUsernameAlreadyUsed()
    {
        var emp = SampleEmployee();
        var (ctx, auth, password, _) = AuthServiceTestFixture.CreateAuthService(emp);
        ctx.UserAccounts.Add(new UserAccount
        {
            Username = "taken",
            Password = password.HashPassword("x"),
            Role = "USER",
            EmployeeId = Guid.NewGuid()
        });
        await ctx.SaveChangesAsync();

        await Assert.ThrowsAsync<Exception>(() =>
            auth.Signup(new SignupRequestDto("taken", "Other1!"), "tok"));
    }

    [Fact(DisplayName = "[Auth — Service] Inscription — l’employé a déjà un compte → exception")]
    public async Task Signup_Throws_WhenEmployeeAlreadyHasAccount()
    {
        var emp = SampleEmployee();
        var (_, auth, _, _) = AuthServiceTestFixture.CreateAuthService(emp);
        await auth.Signup(new SignupRequestDto("u1", "Abcdef1!"), "t1");

        await Assert.ThrowsAsync<Exception>(() =>
            auth.Signup(new SignupRequestDto("u2", "Abcdef1!"), "t2"));
    }

    [Fact(DisplayName = "[Auth — Service] Inscription — jeton employé invalide → exception")]
    public async Task Signup_Throws_WhenEmployeeTokenInvalid()
    {
        var (_, auth, _, _) = AuthServiceTestFixture.CreateAuthService(null);

        await Assert.ThrowsAsync<Exception>(() =>
            auth.Signup(new SignupRequestDto("ghost", "Abcdef1!"), "bad"));
    }

    [Fact(DisplayName = "[Auth — Service] Extraire l’id employé d’un Bearer JWT valide")]
    public async Task GetEmployeeIdFromToken_ReturnsEmployeeId_WhenBearerValid()
    {
        var emp = SampleEmployee();
        var (_, auth, _, jwt) = AuthServiceTestFixture.CreateAuthService(emp);
        await auth.Signup(new SignupRequestDto("dave", "Zzzzzz1!"), "t");
        var token = jwt.GenerateToken("dave", "USER");

        var id = auth.GetEmployeeIdFromToken("Bearer " + token);

        Assert.Equal(emp.Id, id);
    }
}
