using user_account_service.Client;
using user_account_service.DTOs;

namespace my_project_main.Tests.user_account_service.Tests.Support;

internal sealed class StubEmployeeApi : IEmployeeApi
{
    private readonly EmployeeDTO? _employeeForAnyToken;

    public StubEmployeeApi(EmployeeDTO? employeeForAnyToken) =>
        _employeeForAnyToken = employeeForAnyToken;

    public Task<EmployeeDTO> GetEmployeeById(Guid id) =>
        throw new NotSupportedException("Non utilisé dans ces tests.");

    public Task<EmployeeDTO> GetEmployeeByToken(string token) =>
        Task.FromResult(_employeeForAnyToken!);

    public Task VerifyEmployee(Guid id) => Task.CompletedTask;
}
