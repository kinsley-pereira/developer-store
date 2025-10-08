using Microsoft.Extensions.Logging;
using Sales.Application.Interfaces;

namespace Sales.Infra.Events
{
    public class NullEventPublisher(ILogger<NullEventPublisher> logger) : IEventPublisher
    {
        private readonly ILogger<NullEventPublisher> _logger = logger;

        public Task PublishAsync(object evt)
        {
            _logger.LogInformation("Event (not published to broker): {Type}", evt.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
