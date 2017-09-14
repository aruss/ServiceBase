namespace ServiceBase.Events.RabbitMQ
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using global::RabbitMQ.Client;
    using Microsoft.Extensions.Logging;
    using ServiceBase.Logging;

    /// <summary>
    /// Default implementation of the event service. Write events raised to the
    /// log.
    /// </summary>
    public class RabbitMqEventSink : IEventSink
    {
        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Options
        /// </summary>
        private readonly RabbitMqOptions _options;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventSink"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public RabbitMqEventSink(
            ILogger<RabbitMqEventSink> logger,
            RabbitMqOptions options)
        {
            this._logger = logger;
            this._options = options;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evt">The event.</param>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public virtual Task PersistAsync(Event evt)
        {
            if (evt == null)
            {
                throw new ArgumentNullException(nameof(evt));
            }

            var factory = new ConnectionFactory()
            {
                Uri = this._options.Uri                
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: "logs",
                    type: "fanout");

                var message = LogSerializer.Serialize(evt);
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "logs",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);
            }

            return Task.FromResult(0);
        }
    }
}
