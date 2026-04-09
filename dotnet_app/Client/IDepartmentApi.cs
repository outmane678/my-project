using dotnet_app.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;
using Refit;
namespace dotnet_app.Client;

    public interface IDepartmentAPI
    {
        // Méthodes pour interagir avec l'API des départements
        [Get("/api/Departments/get-departement/{id}")]
        Task<DepartmentDto> GetDepartmentById(Guid id);
    }
