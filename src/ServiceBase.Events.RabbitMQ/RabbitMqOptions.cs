namespace ServiceBase.Events.RabbitMQ
{
    using System;

    public class RabbitMqOptions
    {
        public Uri Uri { get; set; }
        public string ExchangeName { get; internal set; } = "logs"; 
    }
}
