using CleanArch.Domain.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Polly;

namespace CleanArch.Infrastructure.Messaging.Configuration;

public class RabbitMqConnection : IRabbitMqConnection, IDisposable
{
    private readonly IConnectionFactory? _connectionFactory;
    private readonly ILogger<RabbitMqConnection> _logger;
    private readonly MessageBrokerOptions _mbOptions;
    private IConnection? _connection;
    private bool _disposed;

    public RabbitMqConnection(
        IConfiguration configuration,
        ILogger<RabbitMqConnection> logger,
        IOptions<MessageBrokerOptions> mbOptions)
    {
        _logger = logger;
        _mbOptions = mbOptions.Value;

        if (!_mbOptions.Enabled)
        {
            _logger.LogWarning("RabbitMQ está deshabilitado por configuración.");
            return;
        }

        _connectionFactory = new ConnectionFactory
        {
            Uri = new Uri(_mbOptions.Host!),
            UserName = _mbOptions.Username,
            Password = _mbOptions.Password,
            DispatchConsumersAsync = true
        };

        TryConnect();
    }

    public bool IsConnected => _connection != null && _connection.IsOpen && !_disposed;

    public IModel? CreateModel()
    {
        if (!IsConnected)
        {
            _logger.LogWarning("Conexión no activa. Reintentando...");
            TryConnect();
        }

        if (!IsConnected)
        {
            _logger.LogError("No se pudo establecer conexión con RabbitMQ.");
        }

        return _connection?.CreateModel();
    }

    private void TryConnect()
    {
        var policy = Policy
            .Handle<BrokerUnreachableException>()
            .Or<SocketException>()
            .WaitAndRetry(
                retryCount: _mbOptions.RetryCount,
                sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(_mbOptions.InitialDelaySeconds, retryAttempt)),
                onRetry: (exception, delay, retry, context) =>
                {
                    _logger.LogWarning(exception,
                        "[{Retry}] Reintentando conexión a RabbitMQ en {Delay}s...",
                        retry, delay.TotalSeconds);
                });

        try
        {
            policy.Execute(() =>
            {
                _connection = _connectionFactory?.CreateConnection();
                _logger.LogInformation("Conexión a RabbitMQ establecida.");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al conectarse a RabbitMQ luego de {RetryCount} reintentos", _mbOptions.RetryCount);
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;

        try
        {
            _connection?.Dispose();
            _logger.LogInformation("Conexión a RabbitMQ cerrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cerrar la conexión a RabbitMQ.");
        }
    }
}