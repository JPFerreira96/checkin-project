using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// TROCAR pelos namespaces reais que aparecem em Employee.cs e AppDbContext.cs
using CheckinProjectBackend.Domain.Entities;          // ← copie do Employee.cs
using CheckinProjectBackend.Infrastructure.Persistence; // ← copie do AppDbContext.cs

namespace CheckinProjectBackend.Controllers; // ← copie do AuthController/WorkController

[ApiController]
[Route("dev")]
public class DevController : ControllerBase
{
    private readonly AppDbContext _db;

    public DevController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost("seed")]
    public async Task<IActionResult> Seed(CancellationToken cancellationToken)
    {
        // Se já tiver funcionários, não duplica
        if (await _db.Employees.AnyAsync(cancellationToken))
        {
            var all = await _db.Employees.ToListAsync(cancellationToken);
            return Ok(new
            {
                message = "Já existem funcionários na base.",
                count = all.Count,
                employees = all.Select(e => new { e.Id, e.Name, e.Email })
            });
        }

        var gestor = new Employee("Gestor Teste", "gestor@empresa.com");
        var func   = new Employee("Funcionário Teste", "funcionario@empresa.com");

        _db.Employees.AddRange(gestor, func);
        await _db.SaveChangesAsync(cancellationToken);

        return Ok(new
        {
            message = "Funcionários criados com sucesso.",
            gestor  = new { gestor.Id, gestor.Name, gestor.Email },
            func    = new { func.Id, func.Name, func.Email }
        });
    }
}
