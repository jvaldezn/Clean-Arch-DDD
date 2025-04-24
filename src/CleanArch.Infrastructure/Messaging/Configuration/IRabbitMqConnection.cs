using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Infrastructure.Messaging.Configuration;

public interface IRabbitMqConnection
{
    IModel? CreateModel();
}