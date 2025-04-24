using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArch.Domain.Messaging;

public class MessageBrokerOptions
{
    public bool Enabled { get; set; }
    public string? Host { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public int RetryCount { get; set; }
    public int InitialDelaySeconds { get; set; }
}