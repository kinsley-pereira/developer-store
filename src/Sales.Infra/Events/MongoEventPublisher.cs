using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Sales.Application.Interfaces;

namespace Sales.Infra.Events
{
    public class MongoEventPublisher : IEventPublisher
    {
        private readonly IMongoCollection<object> _col;
        private readonly ILogger<MongoEventPublisher> _logger;

        public MongoEventPublisher(IMongoClient client, string dbName, ILogger<MongoEventPublisher> logger)
        {
            _col = client.GetDatabase(dbName).GetCollection<object>("events");
            _logger = logger;
        }

        public async Task PublishAsync(object evt)
        {
            _logger.LogInformation("Publishing event to Mongo: {Type}", evt.GetType().Name);
            await _col.InsertOneAsync(evt);
        }
    }
}
