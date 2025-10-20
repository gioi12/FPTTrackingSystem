using DataTranferObjects.Common.Request;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using System.Threading.Tasks;

namespace FPTTrackingSystem.Services.Common.MQ
{
    public class RabbitMQProducer
    {
        private readonly RabbitMQSettings _settings;
        public RabbitMQProducer(IOptions<RabbitMQSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task SendMessage<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(queue: _settings.QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties();

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: _settings.QueueName,
                mandatory: false,
                basicProperties: properties,
                body: body.AsMemory(),
                cancellationToken: default
            );
        }
    }
}
