using CheckinProjectBackend.Domain.Entities;
using CheckinProjectBackend.Domain.Repositories;
using CheckinProjectBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CheckinProjectBackend.Infrastructure.Repositories;

public sealed class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _dbContext;

    public EmployeeRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Employee?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        _dbContext.Employees.FirstOrDefaultAsync(e => e.Email == email, cancellationToken);
}
