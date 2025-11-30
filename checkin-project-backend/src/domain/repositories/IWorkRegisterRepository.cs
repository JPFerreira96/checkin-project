using CheckinProjectBackend.Domain.Common;
using CheckinProjectBackend.Domain.Entities;

namespace CheckinProjectBackend.Domain.Repositories;

public interface IWorkRegisterRepository
{
    Task<WorkRegister?> GetOpenRecordAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        WorkRegister record,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        WorkRegister record,
        CancellationToken cancellationToken = default);

    Task<PagedResult<WorkRegister>> ListAsync(
        string? employeeName,
        DateTime? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
