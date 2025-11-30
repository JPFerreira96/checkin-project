using CheckinProjectBackend.Application.DTOs;
using CheckinProjectBackend.Application.Interfaces;
using CheckinProjectBackend.Domain.Repositories;

namespace CheckinProjectBackend.Application.Services;

public sealed class AuthService : IAuthService
{
    private readonly IEmployeeRepository _employeeRepository;
    
    private readonly Dictionary<string, (string Password, string Role)> _fakeUsers = new()
    {
        ["gestor@empresa.com"]      = ("123456", "manager"),
        ["funcionario@empresa.com"] = ("123456", "employee")
    };

    public AuthService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<LoginResponseDto> LoginAsync(
        LoginRequestDto request,
        CancellationToken cancellationToken = default)
    {
        if (!_fakeUsers.TryGetValue(request.Email, out var info) ||
            info.Password != request.Password)
        {
            throw new UnauthorizedAccessException("Credenciais inválidas.");
        }

        var employee = await _employeeRepository.GetByEmailAsync(request.Email, cancellationToken)
                       ?? throw new UnauthorizedAccessException("Funcionário não encontrado.");

        return new LoginResponseDto(employee.Id, employee.Name, info.Role);
    }
}
