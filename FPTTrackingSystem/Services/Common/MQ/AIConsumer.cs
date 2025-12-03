
using DataTranferObjects.Common.Request;
using DataTranferObjects.Enum;
using FPTTrackingSystem.Services.Admin;
using FPTTrackingSystem.Services.Common.Gemini;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FPTTrackingSystem.Services.Common.MQ
{
    public class AIConsumer : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQSettings _settings;
        private readonly IMemoryCache _cache;
        public AIConsumer(IServiceProvider serviceProvider, IOptions<RabbitMQSettings> settings,IMemoryCache cache)
        {
            _serviceProvider = serviceProvider;
            _settings = settings.Value;
            _cache = cache;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _settings.HostName,
                UserName = _settings.UserName,
                Password = _settings.Password
            };

            var connection = await factory.CreateConnectionAsync();
            var channel = await connection.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: StringEnum.AI_Queue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var task = JsonSerializer.Deserialize<AITaskMessage>(message);

                using var scope = _serviceProvider.CreateScope();
                var aiService = scope.ServiceProvider.GetRequiredService<IGeminiService>();
                var result = await aiService.AskGeminiAsync(task.Prompt?.ToString() ?? "");
                _cache.Set(task.TaskId, result, TimeSpan.FromMinutes(5));

            };

            await channel.BasicConsumeAsync(
                queue: StringEnum.AI_Queue,
                autoAck: true,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}
