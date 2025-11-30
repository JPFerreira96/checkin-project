using CheckinProjectBackend.Domain.Entities;

namespace CheckinProjectBackend.Domain.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}
