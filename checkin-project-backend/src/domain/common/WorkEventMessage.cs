namespace CheckinProjectBackend.Domain.Common;

public sealed class WorkEventMessage
{
    public string EventType { get; init; } = default!; // "checkin" ou "checkout"
    public int EmployeeId { get; init; }
    public string EmployeeName { get; init; } = default!;
    public DateTime CheckinTime { get; init; }
    public DateTime? CheckoutTime { get; init; }
    public double? DurationHours { get; init; }
}