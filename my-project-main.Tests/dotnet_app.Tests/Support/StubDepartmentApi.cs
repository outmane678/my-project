using dotnet_app.Client;
using dotnet_app.DTOs.Requests;

namespace my_project_main.Tests.dotnet_app.Tests.Support;

internal sealed class StubDepartmentApi(DepartmentDto dto) : IDepartmentAPI
{
    public Task<DepartmentDto> GetDepartmentById(Guid id) =>
        Task.FromResult(dto.Id == id ? dto : null!);
}
