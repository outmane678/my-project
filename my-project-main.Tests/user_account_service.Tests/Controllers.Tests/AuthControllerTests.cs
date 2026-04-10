using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using user_account_service.Controllers;
using user_account_service.DTOs;
using user_account_service.DTOs.Requests;
using my_project_main.Tests.user_account_service.Tests.Support;

namespace my_project_main.Tests.user_account_service.Tests.Controllers.Tests;

public class AuthControllerTests
{
    private static EmployeeDTO SampleEmployee() => new()
    {
        Id = Guid.NewGuid(),
        FirstName = "A",
        LastName = "B",
        Phone = "1",
        Position = "P",
        IsVerified = false
    };

    [Fact(DisplayName = "[Auth — API] Inscription (201 Created)")]
    public async Task Signup_Returns201_WhenSucceeds()
    {
        var (_, auth, _, _) = AuthServiceTestFixture.CreateAuthService(SampleEmployee());
        var controller = new AuthController(auth);

        var result = await controller.Signup(new SignupRequestDto("newuser", "Password1!"), "token");

        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status201Created, status.StatusCode);
    }

    [Fact(DisplayName = "[Auth — API] Connexion avec identifiants valides (200 + JWT)")]
    public async Task Login_ReturnsOkWithToken_WhenCredentialsValid()
    {
        var emp = SampleEmployee();
        var (_, auth, _, _) = AuthServiceTestFixture.CreateAuthService(emp);
        await auth.Signup(new SignupRequestDto("loginuser", "Secret123!"), "t");
        var controller = new AuthController(auth);

        var result = await controller.Login(new LoginRequestDto("loginuser", "Secret123!"));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact(DisplayName = "[Auth — API] Récupérer l’id employé depuis le Bearer token (200 OK)")]
    public async Task GetMyEmployeeId_ReturnsOk_WhenBearerTokenValid()
    {
        var emp = SampleEmployee();
        var (_, auth, _, jwt) = AuthServiceTestFixture.CreateAuthService(emp);
        await auth.Signup(new SignupRequestDto("meuser", "Secret123!"), "t");
        var token = jwt.GenerateToken("meuser", "USER");
        var controller = new AuthController(auth);

        var result = controller.GetMyEmployeeId("Bearer " + token);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }
}
