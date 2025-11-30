using CheckinProjectBackend.Domain.Common;
using CheckinProjectBackend.Domain.Entities;
using CheckinProjectBackend.Domain.Repositories;
using CheckinProjectBackend.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CheckinProjectBackend.Infrastructure.Repositories;

public sealed class WorkRegisterRepository : IWorkRegisterRepository
{
    private readonly AppDbContext _dbContext;

    public WorkRegisterRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<WorkRegister?> GetOpenRecordAsync(
        int employeeId,
        CancellationToken cancellationToken = default) =>
        _dbContext.WorkRegisters
            .Include(w => w.Employee)
            .Where(w => w.EmployeeId == employeeId && w.CheckoutTime == null)
            .OrderByDescending(w => w.CheckinTime)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task AddAsync(WorkRegister record, CancellationToken cancellationToken = default)
    {
        await _dbContext.WorkRegisters.AddAsync(record, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkRegister record, CancellationToken cancellationToken = default)
    {
        _dbContext.WorkRegisters.Update(record);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PagedResult<WorkRegister>> ListAsync(
        string? employeeName,
        DateTime? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.WorkRegisters
            .Include(w => w.Employee)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(employeeName))
        {
            query = query.Where(w => w.Employee.Name.Contains(employeeName));
        }

        if (date.HasValue)
        {
            // date vem da query string como Unspecified -> marcamos explicitamente como UTC
            var dayLocal   = date.Value.Date; // 2025-11-29 00:00:00 (Kind = Unspecified)
            var dayStartUtc = DateTime.SpecifyKind(dayLocal, DateTimeKind.Utc);
            var dayEndUtc   = dayStartUtc.AddDays(1);

            query = query.Where(w =>
                w.CheckinTime >= dayStartUtc &&
                w.CheckinTime <  dayEndUtc);
        }

        var totalItems = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(w => w.CheckinTime)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<WorkRegister>(items, totalItems, page, pageSize);
    }
}
