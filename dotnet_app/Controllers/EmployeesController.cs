using dotnet_app.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
using dotnet_app.Services.DTOs.Requests;
namespace dotnet_app.Controllers;
using dotnet_app.Models.Entities;
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly EmployeeServiceImp _employeeService;

    public EmployeeController(EmployeeServiceImp employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpPost("create-employee")]
    public async Task<IActionResult> CreateEmployee([FromBody] EmployeeCreate employeeCreate)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try{
       var employee = await _employeeService.CreateEmployee(employeeCreate);
       return Ok(employee);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // récupérer tous les employés
    [HttpGet("get-all-employees")]
    public async Task<IActionResult> GetAllEmployees()
    {
        try
        {
            var employees = await _employeeService.GetAllEmployees();
            return Ok(employees);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    // récupérer un employé par son ID
    [HttpGet("get-employee/{id}")]
    public async Task<IActionResult> GetEmployeeById(Guid id)
    { 
        try
        {
            Employee employee = await _employeeService.GetEmployeeById(id);
            return Ok(employee);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }


    // supprimer un employé par son ID
    [HttpDelete("delete-employee/{id}")]
    public async Task<IActionResult> DeleteEmployee(Guid id)
    {
        try
        {
            await _employeeService.DeleteEmployee(id);
            return NoContent();
        }
        catch (Exception ex)
        {
           return NotFound(ex.Message);
        }
    }


    // mettre à jour un employé
    [HttpPut("update-employee/{id}")]
    public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] EmployeeUpdate employeeUpdate)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Employee employee = await _employeeService.UpdateEmployee(id, employeeUpdate);
            return Ok(employee);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpGet("get-employee-by-token/{token}")]
    public async Task<IActionResult> GetEmployeeByToken(string token)
    {
        try
        {
            Employee employee = await _employeeService.GetEmployeeByTokenAsync(token);
            return Ok(employee);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpPost("verify-employee/{id}")]
    public async Task<IActionResult> VerifyEmployee(Guid id)
    {
        try
        {
            await _employeeService.VerifyEmployeeAsync(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}