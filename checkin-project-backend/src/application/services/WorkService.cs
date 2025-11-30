using CheckinProjectBackend.Application.Dtos;
using CheckinProjectBackend.Application.Interfaces;
using CheckinProjectBackend.Domain.Common;
using CheckinProjectBackend.Domain.Entities;
using CheckinProjectBackend.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace CheckinProjectBackend.Application.Services;

public sealed class WorkService : IWorkService
{
    private readonly IWorkRegisterRepository _workRegisterRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<WorkService> _logger;

    public WorkService(
        IWorkRegisterRepository workRegisterRepository,
        IEmployeeRepository employeeRepository,
        ILogger<WorkService> logger,
        IWorkEventPublisher workEventPublisher
    )
    {
        _workRegisterRepository = workRegisterRepository;
        _employeeRepository = employeeRepository;
        _logger = logger;
        _workEventPublisher = workEventPublisher;
    }

    // public async Task<WorkRegisterDto> CheckInAsync(int employeeId, CancellationToken cancellationToken = default)
    // {
    //     _logger.LogInformation("Check-in solicitado para funcionário {EmployeeId}", employeeId);

    //     var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken)
    //                    ?? throw new InvalidOperationException("Funcionário não encontrado.");

    //     var open = await _workRegisterRepository.GetOpenRecordAsync(employeeId, cancellationToken);
    //     if (open is not null)
    //         throw new InvalidOperationException("Já existe um check-in em aberto.");

    //     var record = new WorkRegister(employeeId, DateTime.UtcNow);
    //     await _workRegisterRepository.AddAsync(record, cancellationToken);

    //     return MapToDto(record, employee.Name);
    // }

    public async Task<WorkRegisterDto> RegisterCheckinAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        // Carrega funcionário pelo e-mail
        var employee =
            await _employeeRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Funcionário não encontrado.");

        var nowUtc = DateTime.UtcNow;

        var record = new WorkRegister
        {
            EmployeeId = employee.Id,
            CheckinTime = nowUtc,
            CheckoutTime = null,
            Duration = null,
        };

        await _workRegisterRepository.AddAsync(record, cancellationToken);

        // garante que o Employee está preenchido para o DTO
        record.Employee = employee;

        // → PUBLICA EVENTO NO RABBITMQ
        var evt = new WorkEventMessage
        {
            EventType = "checkin",
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            CheckinTime = record.CheckinTime,
            CheckoutTime = null,
            DurationHours = null,
        };
        await _workEventPublisher.PublishAsync(evt, cancellationToken);

        return WorkRegisterDto.FromEntity(record);
    }

    public async Task<WorkRegisterDto> CheckInAsync(
        int employeeId,
        CancellationToken cancellationToken
    )
    {
        var nowUtc = DateTime.UtcNow;

        var record = new WorkRegister
        {
            EmployeeId = employeeId,
            CheckinTime = nowUtc,
            CheckoutTime = null,
            DurationHours = null,
        };

        await _workRegisterRepository.AddAsync(record, cancellationToken);
        return WorkRegisterDto.FromEntity(record);
    }

    public async Task<WorkRegisterDto> CheckOutAsync(
        int employeeId,
        CancellationToken cancellationToken
    )
    {
        var nowUtc = DateTime.UtcNow;

        var record = await _workRegisterRepository.GetOpenRecordAsync(
            employeeId,
            cancellationToken
        );

        if (record is null)
            throw new InvalidOperationException("Nenhum registro em aberto para este funcionário.");

        record.CheckoutTime = nowUtc;
        record.DurationHours = (nowUtc - record.CheckinTime).TotalHours;

        await _workRegisterRepository.UpdateAsync(record, cancellationToken);
        return WorkRegisterDto.FromEntity(record);
    }

    public async Task<IReadOnlyList<WorkRegisterDto>> ListAsync(
        string? name,
        DateTime? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default
    )
    {
        var pagedRegisters = await _workRegisterRepository.ListAsync(
            name,
            date,
            page,
            pageSize,
            cancellationToken
        );

        return pagedRegisters.Items.Select(WorkRegisterDto.FromEntity).ToList();
    }

    public async Task<WorkRegisterDto?> RegisterCheckoutAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        var employee =
            await _employeeRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Funcionário não encontrado.");

        var openRecord = await _workRegisterRepository.GetOpenRecordAsync(
            employee.Id,
            cancellationToken
        );

        if (openRecord is null)
        {
            return null; // não há registro aberto
        }

        var nowUtc = DateTime.UtcNow;

        openRecord.CheckoutTime = nowUtc;
        openRecord.Duration = nowUtc - openRecord.CheckinTime;

        await _workRegisterRepository.UpdateAsync(openRecord, cancellationToken);

        var evt = new WorkEventMessage
        {
            EventType = "checkout",
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            CheckinTime = openRecord.CheckinTime,
            CheckoutTime = openRecord.CheckoutTime,
            DurationHours = openRecord.Duration?.TotalHours,
        };
        await _workEventPublisher.PublishAsync(evt, cancellationToken);

        return WorkRegisterDto.FromEntity(openRecord);
    }

    private static WorkRegisterDto MapToDto(WorkRegister r, string employeeName) =>
        new(r.Id, employeeName, r.CheckinTime, r.CheckoutTime, r.Duration?.TotalHours);
}
