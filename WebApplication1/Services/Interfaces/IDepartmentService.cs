using WebApplication1.Models;

namespace WebApplication1.Services.Interfaces
{
    public interface DepartmentService
    {
        Department CreateDepartment(DepartmentCreate department);
        Department UpadateDepartment(Guid id, DepartmentUpdate department);
        void DeleteDepartment(Guid id);
        Department FindDepartmentById(Guid id);
        List<Department> GetAllDepartments();
    }
}