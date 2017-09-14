namespace ServiceBase.Events.RabbitMQ
{
    using System;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
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
        /// <param name="evnt">The event.</param>
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

                channel.BasicPublish(exchange: this._options.ExchangeName,
                                     routingKey: String.Empty,
                                     basicProperties: null,
                                     body: ToByteArray(evnt));

                this._logger.LogInformation(LogSerializer.Serialize(evnt));
            }

            return Task.FromResult(0);
        }

        /// <summary>
        /// Converts objec to binary array
        /// </summary>
        /// <param name="obj">Object to convert</param>
        /// <returns>Byte array</returns>
        public byte[] ToByteArray<TObject>(TObject obj) where TObject : class
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
