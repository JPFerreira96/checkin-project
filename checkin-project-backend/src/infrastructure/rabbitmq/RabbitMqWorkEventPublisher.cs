using System.Text;
using System.Text.Json;
using CheckinProjectBackend.Domain.Common;
using CheckinProjectBackend.Domain.Repositories;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace CheckinProjectBackend.Infrastructure.RabbitMq;

public sealed class RabbitMqWorkEventPublisher : IWorkEventPublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly RabbitMqOptions _options;
    private IModel? _channel;

    public RabbitMqWorkEventPublisher(
        IOptions<RabbitMqOptions> options,
        IConnection connection)
    {
        _options = options.Value;
        _connection = connection;
    }

    private IModel Channel
    {
        get
        {
            if (_channel is { IsClosed: false })
                return _channel;

            _channel = _connection.CreateModel();
            _channel.QueueDeclare(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            return _channel;
        }
    }

    public Task PublishAsync(WorkEventMessage message, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        var props = Channel.CreateBasicProperties();
        props.DeliveryMode = 2; // persistente

        Channel.BasicPublish(
            exchange: "",
            routingKey: _options.QueueName,
            basicProperties: props,
            body: body);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
    }
}
