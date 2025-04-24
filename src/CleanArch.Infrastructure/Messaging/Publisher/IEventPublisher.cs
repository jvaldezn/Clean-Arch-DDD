namespace CleanArch.Infrastructure.Messaging.Publisher;

public interface IEventPublisher
{
    Task Publish<T>(T message) where T : class;
}
