namespace ServiceBase.Events.RabbitMQ
{
    using System;
    using System.Threading.Tasks;
    using global::RabbitMQ.Client;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Write events to RabbitMQ by using MessagePack serializer.
    /// </summary>
    public class RabbitMqEventSink : IEventSink
    {
        private readonly ILogger _logger;
        private readonly IBinarySerializer _serializer;
        private readonly RabbitMqOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RabbitMqEventSink"/>
        /// class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public RabbitMqEventSink(
            ILogger<RabbitMqEventSink> logger,
            IBinarySerializer serializer,
            RabbitMqOptions options)
        {
            this._logger = logger;
            this._serializer = serializer;
            this._options = options;
        }

        /// <summary>
        /// Raises the specified event.
        /// </summary>
        /// <param name="evnt">Instance of <see cref="Event"/></param>
        /// <exception cref="System.ArgumentNullException">evt</exception>
        public virtual Task PersistAsync(Event evnt)
        {
            if (evnt == null)
            {
                throw new ArgumentNullException(nameof(evnt));
            }

            var factory = new ConnectionFactory()
            {
                Uri = this._options.Uri
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: this._options.ExchangeName,
                    type: ExchangeType.Fanout);

                byte[] body = this._serializer.Serialize(evnt);

                channel.BasicPublish(
                    exchange: this._options.ExchangeName,
                    routingKey: String.Empty,
                    basicProperties: null,
                    body: body
                );
            }

            return Task.CompletedTask;
        }
    }
}