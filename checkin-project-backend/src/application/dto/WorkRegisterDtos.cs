using CheckinProjectBackend.Domain.Entities;

namespace CheckinProjectBackend.Application.Dtos;

public sealed class WorkRegisterDto
{
    public int Id { get; }
    public string EmployeeName { get; }
    public DateTime CheckinTime { get; }
    public DateTime? CheckoutTime { get; }
    public double? DurationHours { get; }

    public WorkRegisterDto(
        int id,
        string employeeName,
        DateTime checkinTime,
        DateTime? checkoutTime,
        double? durationHours)
    {
        Id            = id;
        EmployeeName  = employeeName;
        CheckinTime   = checkinTime;
        CheckoutTime  = checkoutTime;
        DurationHours = durationHours;
    }

    public static WorkRegisterDto FromEntity(WorkRegister entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new WorkRegisterDto(
            entity.Id,
            entity.Employee?.Name ?? string.Empty,
            entity.CheckinTime,
            entity.CheckoutTime,
            entity.Duration?.TotalHours
        );
    }
}
