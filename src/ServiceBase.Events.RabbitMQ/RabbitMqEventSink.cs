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
        private readonly ILogger logger;
        private readonly IBinarySerializer serializer;
        private readonly RabbitMqOptions options;

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
            this.logger = logger;
            this.serializer = serializer;
            this.options = options;
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

            ConnectionFactory factory = new ConnectionFactory()
            {
                Uri = this.options.Uri
            };

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(
                    exchange: this.options.ExchangeName,
                    type: ExchangeType.Fanout);

                byte[] body = this.serializer.Serialize(evnt);

                channel.BasicPublish(
                    exchange: this.options.ExchangeName,
                    routingKey: String.Empty,
                    basicProperties: null,
                    body: body
                );
            }

            return Task.CompletedTask;
        }
    }
}