using Microsoft.AspNetCore.Mvc;
namespace WebApplication1.Controllers;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Implementations;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly DepartmentServiceImp _departmentService;
    public DepartmentsController(DepartmentServiceImp departmentService)
    {
        _departmentService = departmentService;
    }
      
    // créer un département
    [HttpPost("create-departement")]  
    public ActionResult<Department> CreateDepartment([FromBody] DepartmentCreate departmentCreate)
    {
        // Appeler le service pour créer le département
        var department = _departmentService.CreateDepartment(departmentCreate);
        return Ok(department);
    }
    
    // mettre à jour un département
    [HttpPut("update-departement/{id}")]
    public ActionResult<Department> UpdateDepartment(Guid id, [FromBody] DepartmentUpdate departmentUpdate)
    {
        try
        {
            var updatedDepartment = _departmentService.UpadateDepartment(id, departmentUpdate);
            return Ok(updatedDepartment);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }

    // supprimer un département
    [HttpDelete("delete-departement/{id}")]
    public ActionResult DeleteDepartment(Guid id)
    {
        try
        {
            _departmentService.DeleteDepartment(id);
            return NoContent();
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }


    // trouver un département par son id
    [HttpGet("get-departement/{id}")]
    public ActionResult<Department> GetDepartmentById(Guid id)
    {
        var department = _departmentService.FindDepartmentById(id);
        if(department == null)
        {
            return NotFound($"Le département avec l'ID {id} n'a pas été trouvé !");
        }
        return Ok(department);
    }

    // afficher tous les départements 
    [HttpGet("get-all-departments")]
    public IActionResult GetAllDepartments()
    {
        var departements = _departmentService.GetAllDepartments();
        return Ok(departements);
    }


}