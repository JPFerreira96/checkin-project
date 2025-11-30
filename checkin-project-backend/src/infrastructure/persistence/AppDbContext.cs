using CheckinProjectBackend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CheckinProjectBackend.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Employee> Employees   => Set<Employee>();
    public DbSet<WorkRegister> WorkRegisters => Set<WorkRegister>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("employees");

            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name").IsRequired();
            entity.Property(e => e.Email).HasColumnName("email").IsRequired();
        });

        modelBuilder.Entity<WorkRegister>(entity =>
        {
            entity.ToTable("work_records");

            entity.HasKey(w => w.Id);
            entity.Property(w => w.Id).HasColumnName("id");
            entity.Property(w => w.EmployeeId).HasColumnName("employee_id");
            entity.Property(w => w.CheckinTime).HasColumnName("checkin_time").IsRequired();
            entity.Property(w => w.CheckoutTime).HasColumnName("checkout_time");
            entity.Property(w => w.Duration).HasColumnName("duration");
        });
    }
}
