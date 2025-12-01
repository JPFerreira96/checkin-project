using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CheckinProjectBackend.Domain.Entities;
using CheckinProjectBackend.Infrastructure.Persistence;

namespace CheckinProjectBackend.Controllers;

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
