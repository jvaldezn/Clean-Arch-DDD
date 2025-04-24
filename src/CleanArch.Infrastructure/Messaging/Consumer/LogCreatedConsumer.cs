using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Messaging;
using CleanArch.Domain.Products;
using CleanArch.Domain.Shared;
using CleanArch.Infrastructure.Context;
using CleanArch.Infrastructure.Messaging.Configuration;
using CleanArch.Infrastructure.Messaging.Contract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace CleanArch.Infrastructure.Messaging.Consumer;

public class LogCreatedConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LogCreatedConsumerService> _logger;
    private readonly IRabbitMqConnection _rabbitMqConnection;
    private readonly RabbitMQOptions _rmOptions;

    public LogCreatedConsumerService(IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<LogCreatedConsumerService> logger,
        IRabbitMqConnection rabbitMqConnection,
        IOptions<RabbitMQOptions> rmOptions)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
        _rabbitMqConnection = rabbitMqConnection;
        _rmOptions = rmOptions.Value;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var isEnabled = _configuration.GetValue<bool>("MessageBroker:Enabled");
        if (!isEnabled)
        {
            _logger.LogWarning("RabbitMQ está deshabilitado por configuración.");
            return Task.CompletedTask;
        }

        var channel = _rabbitMqConnection.CreateModel();

        if (channel == null)
        {
            _logger.LogWarning("No se pudo iniciar el consumidor porque RabbitMQ no está disponible.");
            return Task.CompletedTask;
        }

        channel.ExchangeDeclare(exchange: _rmOptions.Exchange, type: ExchangeType.Direct, durable: true);
        channel.QueueDeclare(queue: _rmOptions.Queue, durable: true, exclusive: false, autoDelete: false);
        channel.QueueBind(queue: _rmOptions.Queue, exchange: _rmOptions.Exchange, routingKey: _rmOptions.RoutingKey);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.Received += async (model, ea) => await ProcessMessage(ea);

        channel.BasicConsume(queue: _rmOptions.Queue, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private async Task ProcessMessage(BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = JsonSerializer.Deserialize<LogCreatedEvent>(Encoding.UTF8.GetString(body))!;

        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork<LogDbContext>>();
        var logRepository = scope.ServiceProvider.GetRequiredService<ILogRepository>();

        try
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();

            var log = new Log
            {
                MachineName = message.MachineName,
                Logged = message.Logged,
                Level = message.Level,
                Message = message.Message,
                Logger = message.Logger,
                Properties = message.Properties,
                Callsite = message.Callsite,
                Exception = message.Exception,
                ApplicationId = message.ApplicationId
            };

            await logRepository.AddAsync(log);
            await unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            _logger.LogInformation("Mensaje procesado correctamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar el mensaje");
        }
    }
}