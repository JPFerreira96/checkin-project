namespace CheckinProjectBackend.Domain.Common;

public sealed class RabbitMqOptions
{
    public string HostName { get; init; } = "rabbitmq";
    public string UserName { get; init; } = "guest";
    public string Password { get; init; } = "guest";
    public string QueueName { get; init; } = "work-events";
}
