using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CheckinProjectBackend.Domain.Entities;

public sealed class WorkRegister
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public Employee Employee { get; set; } = null!;

    public DateTime CheckinTime { get; set; }

    public DateTime? CheckoutTime { get; set; }

    // Mapeado no PostgreSQL como INTERVAL
    public TimeSpan? Duration { get; set; }

    // Campo auxiliar para facilitar DTO/front – NÃO vai para o banco
    [NotMapped]
    public double? DurationHours
    {
        get => Duration?.TotalHours;
        set
        {
            if (value is null)
            {
                Duration = null;
                return;
            }

            Duration = TimeSpan.FromHours(value.Value);
        }
    }
}
