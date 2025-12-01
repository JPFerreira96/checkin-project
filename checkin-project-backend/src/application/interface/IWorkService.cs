using CheckinProjectBackend.Application.Dtos;

namespace CheckinProjectBackend.Application.Interfaces;

public interface IWorkService
{
    Task<WorkRegisterDto> CheckInAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<WorkRegisterDto> CheckOutAsync(
        int employeeId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<WorkRegisterDto>> ListAsync(
        string? name,
        DateTime? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<WorkRegisterDto> RegisterCheckinAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<WorkRegisterDto?> RegisterCheckoutAsync(
        string email,
        CancellationToken cancellationToken = default);
}
