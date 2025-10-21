
using DataTranferObjects.Common.Request;
using FPTTrackingSystem.Services.Common.Interfaces;
using Microsoft.Extensions.Options;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FPTTrackingSystem.Services.Common.MQ
{
    public class RabbitMQConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _settings;

        public RabbitMQConsumer(IServiceProvider serviceProvider, IOptions<RabbitMQSettings> settings)
        {
            _serviceProvider = serviceProvider;
            _settings = settings.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: _settings.QueueName,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var mailRequest = JsonSerializer.Deserialize<List<MailRequest>>(message);

                using var scope = _serviceProvider.CreateScope();
                var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();
                await mailService.SendEmailAsync(mailRequest);
            };

            await channel.BasicConsumeAsync(
                queue: _settings.QueueName,
                autoAck: true,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}
