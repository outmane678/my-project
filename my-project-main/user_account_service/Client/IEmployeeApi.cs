using Refit;
using user_account_service.DTOs;

namespace user_account_service.Client;

public interface IEmployeeApi
{
    [Get("/api/Employee/get-employee/{id}")] 
    Task<EmployeeDTO> GetEmployeeById(Guid id);

    [Get("/api/Employee/get-employee-by-token/{token}")] 
    Task<EmployeeDTO> GetEmployeeByToken(string token);

    [Post("/api/Employee/verify-employee/{id}")]  
    Task VerifyEmployee(Guid id);
}