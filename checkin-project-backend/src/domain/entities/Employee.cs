namespace CheckinProjectBackend.Domain.Entities;

public class Employee
{
    public int Id { get; private set; }
    public string Name  { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    private readonly List<WorkRegister> _workRegisters = new();
    public IReadOnlyCollection<WorkRegister> WorkRegisters => _workRegisters.AsReadOnly();

    protected Employee() { } // EF

    public Employee(string name, string email)
    {
        Name  = name;
        Email = email;
    }
}