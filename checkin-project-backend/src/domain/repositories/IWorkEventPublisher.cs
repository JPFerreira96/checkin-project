using CheckinProjectBackend.Domain.Common;

namespace CheckinProjectBackend.Domain.Repositories;

public interface IWorkEventPublisher
{
    Task PublishAsync(WorkEventMessage message, CancellationToken cancellationToken = default);
}