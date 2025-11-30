using CheckinProjectBackend.Application.Interfaces;
using CheckinProjectBackend.Application.Services;
using CheckinProjectBackend.Domain.Repositories;
using CheckinProjectBackend.Domain.Common;
using CheckinProjectBackend.Infrastructure.Persistence;
using CheckinProjectBackend.Infrastructure.Repositories;
using CheckinProjectBackend.Infrastructure.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

const string FrontendCorsPolicy = "FrontendCorsPolicy";

var connectionString = builder.Configuration.GetConnectionString("Default");

builder.Services.Configure<RabbitMqOptions>(
    builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddSingleton<IConnection>(sp =>
{
    var options = sp.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

    var factory = new ConnectionFactory()
    {
        HostName = options.HostName,
        UserName = options.UserName,
        Password = options.Password
    };

    return factory.CreateConnection();
});

builder.Services.AddSingleton<IWorkEventPublisher, RabbitMqWorkEventPublisher>();

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
                "http://127.0.0.1:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(FrontendCorsPolicy);
app.UseHttpsRedirection();
app.UseAuthorization();

// Health rápido
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapControllers();

app.Run();

