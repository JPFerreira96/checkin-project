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
    private readonly IWorkEventPublisher _workEventPublisher;

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

    // CHECK-IN POR EMAIL
    public async Task<WorkRegisterDto> RegisterCheckinAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        var employee = await _employeeRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Funcionário não encontrado.");

        var openRecord = await _workRegisterRepository.GetOpenRecordAsync(employee.Id, cancellationToken);
        if (openRecord is not null)
            throw new InvalidOperationException("Já existe um check-in em aberto para este funcionário.");

        var nowUtc = DateTime.UtcNow;

        var record = new WorkRegister
        {
            EmployeeId = employee.Id,
            CheckinTime = nowUtc,
            CheckoutTime = null,
            Duration = null
        };

        await _workRegisterRepository.AddAsync(record, cancellationToken);

        await _workEventPublisher.PublishAsync(new WorkEventMessage
        {
            EventType = "checkin",
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            CheckinTime = nowUtc
        }, cancellationToken);

        return WorkRegisterDto.FromEntity(record);
    }

    // CHECK-IN POR ID
    public async Task<WorkRegisterDto> CheckInAsync(
        int employeeId,
        CancellationToken cancellationToken
    )
    {
        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken)
            ?? throw new InvalidOperationException("Funcionário não encontrado.");

        var openRecord = await _workRegisterRepository.GetOpenRecordAsync(employeeId, cancellationToken);
        if (openRecord is not null)
            throw new InvalidOperationException("Já existe um check-in em aberto para este funcionário.");

        var nowUtc = DateTime.UtcNow;

        var record = new WorkRegister
        {
            EmployeeId = employeeId,
            CheckinTime = nowUtc,
            CheckoutTime = null,
            Duration = null
        };

        await _workRegisterRepository.AddAsync(record, cancellationToken);

        await _workEventPublisher.PublishAsync(new WorkEventMessage
        {
            EventType = "checkin",
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            CheckinTime = nowUtc
        }, cancellationToken);

        return WorkRegisterDto.FromEntity(record);
    }

    // CHECK-OUT POR ID
    public async Task<WorkRegisterDto> CheckOutAsync(
        int employeeId,
        CancellationToken cancellationToken
    )
    {
        var nowUtc = DateTime.UtcNow;

        var record = await _workRegisterRepository.GetOpenRecordAsync(employeeId, cancellationToken);
        if (record is null)
            throw new InvalidOperationException("Nenhum check-in em aberto para este funcionário.");

        record.CheckoutTime = nowUtc;
        record.Duration = nowUtc - record.CheckinTime;

        await _workRegisterRepository.UpdateAsync(record, cancellationToken);

        var employee = await _employeeRepository.GetByIdAsync(employeeId, cancellationToken);

        await _workEventPublisher.PublishAsync(new WorkEventMessage
        {
            EventType = "checkout",
            EmployeeId = employee!.Id,
            EmployeeName = employee.Name,
            CheckinTime = record.CheckinTime,
            CheckoutTime = record.CheckoutTime,
            DurationHours = record.Duration?.TotalHours
        }, cancellationToken);

        return WorkRegisterDto.FromEntity(record);
    }

    // LISTAGEM PAGINADA
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

    // CHECK-OUT POR EMAIL
    public async Task<WorkRegisterDto?> RegisterCheckoutAsync(
        string email,
        CancellationToken cancellationToken = default
    )
    {
        var employee = await _employeeRepository.GetByEmailAsync(email, cancellationToken)
            ?? throw new InvalidOperationException("Funcionário não encontrado.");

        var openRecord = await _workRegisterRepository.GetOpenRecordAsync(employee.Id, cancellationToken);
        if (openRecord is null)
            return null;

        var nowUtc = DateTime.UtcNow;

        openRecord.CheckoutTime = nowUtc;
        openRecord.Duration = nowUtc - openRecord.CheckinTime;

        await _workRegisterRepository.UpdateAsync(openRecord, cancellationToken);

        await _workEventPublisher.PublishAsync(new WorkEventMessage
        {
            EventType = "checkout",
            EmployeeId = employee.Id,
            EmployeeName = employee.Name,
            CheckinTime = openRecord.CheckinTime,
            CheckoutTime = openRecord.CheckoutTime,
            DurationHours = openRecord.Duration?.TotalHours
        }, cancellationToken);

        return WorkRegisterDto.FromEntity(openRecord);
    }
}
