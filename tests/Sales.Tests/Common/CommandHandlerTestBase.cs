using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Sales.Application.Interfaces;
using Sales.Application.Mapping;

namespace Sales.Tests.Common
{
    public abstract class CommandHandlerTestBase
    {
        protected readonly Mock<IAppDbContext> DbMock;
        protected readonly Mock<IMapper> MapperMock;
        protected readonly Mock<ILogger<object>> LoggerMock;
        protected readonly Mock<IEventPublisher> EventPublisherMock;

        protected CommandHandlerTestBase()
        {
            DbMock = new Mock<IAppDbContext>();
            MapperMock = new Mock<IMapper>();
            LoggerMock = new Mock<ILogger<object>>();
            EventPublisherMock = new Mock<IEventPublisher>();
        }

        protected static IMapper CreateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new SalesProfile());
            });
            return config.CreateMapper();
        }
    }
}
