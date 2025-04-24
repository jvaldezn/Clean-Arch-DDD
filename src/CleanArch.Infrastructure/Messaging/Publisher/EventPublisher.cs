using CleanArch.Domain.Messaging;
using CleanArch.Infrastructure.Messaging.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace CleanArch.Infrastructure.Messaging.Publisher;

public class EventPublisher : IEventPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EventPublisher> _logger;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private readonly RabbitMQOptions _rmOptions;

    public EventPublisher(IConfiguration configuration,
        ILogger<EventPublisher> logger,
        IRabbitMqConnection rabbitMqConnection,
        IOptions<RabbitMQOptions> rmOptions)
    {
        _configuration = configuration;
        _logger = logger;
        _rabbitMqConnection = rabbitMqConnection;
        _rmOptions = rmOptions.Value;
    }

    public Task Publish<T>(T message) where T : class
    {
        var isEnabled = _configuration.GetValue<bool>("MessageBroker:Enabled");
        if (!isEnabled)
        {
            _logger.LogWarning("RabbitMQ está deshabilitado por configuración.");
            return Task.CompletedTask;
        }

        try
        {
            var channel = _rabbitMqConnection.CreateModel();

            if (channel == null)
            {
                _logger.LogWarning("No se pudo publicar el evento porque RabbitMQ no está disponible.");
                return Task.CompletedTask;
            }

            channel.ExchangeDeclare(exchange: _rmOptions.Exchange, type: ExchangeType.Direct, durable: true);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: _rmOptions.Exchange, routingKey: _rmOptions.RoutingKey, basicProperties: null, body: body);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al publicar el mensaje de tipo {EventType}", typeof(T).Name);
        }

        return Task.CompletedTask;
    }
}
