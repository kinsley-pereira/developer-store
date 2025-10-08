namespace Sales.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync(object evt);
    }
}
