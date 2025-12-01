using CheckinProjectBackend.Application.Interfaces;
using CheckinProjectBackend.Application.Services;
using CheckinProjectBackend.Domain.Repositories;
using CheckinProjectBackend.Domain.Common;
using CheckinProjectBackend.Infrastructure.Persistence;
using CheckinProjectBackend.Infrastructure.Repositories;
using CheckinProjectBackend.Infrastructure.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<IWorkEventPublisher>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>();
    var logger = sp.GetRequiredService<ILogger<RabbitMqWorkEventPublisher>>();

    try
    {
        var config = options.Value;
        var factory = new ConnectionFactory()
        {
            HostName = config.HostName,
            UserName = config.UserName,
            Password = config.Password
        };

        var connection = factory.CreateConnection();
        return new RabbitMqWorkEventPublisher(options, connection);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "RabbitMQ not reachable, falling back to no-op publisher.");
        return new NoopWorkEventPublisher();
    }
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IWorkRegisterRepository, WorkRegisterRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWorkService, WorkService>();

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: FrontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://127.0.0.1:5173",
                "https://checkin-project-ashy.vercel.app"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Check-in API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors(FrontendCorsPolicy);
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();
