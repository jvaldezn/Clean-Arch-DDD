using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Domain.Messaging;

public class RabbitMQOptions
{
    public string? Exchange { get; set; }
    public string? RoutingKey { get; set; }
    public string? Queue { get; set; }
}