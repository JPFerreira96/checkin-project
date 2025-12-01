using CheckinProjectBackend.Domain.Common;
using CheckinProjectBackend.Domain.Repositories;

namespace CheckinProjectBackend.Infrastructure.RabbitMq;

public sealed class NoopWorkEventPublisher : IWorkEventPublisher
{
    public Task PublishAsync(WorkEventMessage message, CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
