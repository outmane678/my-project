using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using user_account_service.DTOs.Requests;
using user_account_service.Services.Implementations;
using System;

namespace user_account_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Signup avec token d'activation envoyé par email depuis employee-service
    /// </summary>
    [HttpPost("signup")]
    public async Task<IActionResult> Signup(
        [FromBody] SignupRequestDto signupRequest,
        [FromQuery] string token)
    {
        try
        {
            await _authService.Signup(signupRequest, token);
            return StatusCode(StatusCodes.Status201Created, new { message = "Compte créé avec succès !" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Login → retourne directement le token JWT
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto loginRequest)
    {
        try
        {
            var jwt = await _authService.Login(loginRequest);
            return Ok(new { 
                token = jwt,
                type = "Bearer"
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { error = "Identifiants invalides" });
        }
        catch (Exception ex)
        {
            // Log l'erreur
            Console.WriteLine($"Erreur lors de l'authentification: {ex}");
            return StatusCode(StatusCodes.Status500InternalServerError, 
                new { error = "Erreur lors de l'authentification" });
        }
    }

    /// <summary>
    /// Récupérer l'employeeId de l'utilisateur connecté
    /// </summary>
    [Authorize]
    [HttpGet("me/employee-id")]
    public IActionResult GetMyEmployeeId(
        [FromHeader(Name = "Authorization")] string authHeader)
    {
        try
        {
            var employeeId = _authService.GetEmployeeIdFromToken(authHeader);
            return Ok(new { employeeId = employeeId.ToString() });
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}