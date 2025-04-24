using CleanArch.Domain.Messaging;
using CleanArch.Infrastructure.Messaging.Configuration;
using CleanArch.Infrastructure.Messaging.Consumer;
using CleanArch.Infrastructure.Messaging.Publisher;

namespace CleanArch.API.Extensions
{
    public static class RabbitMQExtensions
    {
        public static IServiceCollection AddRabbitMQ(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MessageBrokerOptions>(configuration.GetSection("MessageBroker"));
            services.Configure<RabbitMQOptions>(configuration.GetSection("RabbitMQ"));

            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddHostedService<LogCreatedConsumerService>();
            services.AddSingleton<IEventPublisher, EventPublisher>();

            return services;
        }
    }
}
