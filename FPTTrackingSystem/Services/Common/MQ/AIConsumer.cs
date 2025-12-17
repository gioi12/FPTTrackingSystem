
using DataTranferObjects.Common.Request;
using DataTranferObjects.Common.Response;
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
using System.Threading.Tasks;

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
                var taskMessage = JsonSerializer.Deserialize<AITaskMessage>(message);

                if (taskMessage == null)
                {
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                    return;
                }

                try
                {
                    // 🔹 set Processing
                    _cache.Set(taskMessage.TaskId, new AITaskState
                    {
                        TaskId = taskMessage.TaskId,
                        Status = AIEnum.Processing,
                        CreatedAt = DateTime.UtcNow
                    });

                    using var scope = _serviceProvider.CreateScope();
                    var aiService = scope.ServiceProvider.GetRequiredService<IGeminiService>();

                    var result = await aiService.AskGeminiAsync(taskMessage.Prompt ?? "");

                    // 🔹 Done
                    _cache.Set(taskMessage.TaskId, new AITaskState
                    {
                        TaskId = taskMessage.TaskId,
                        Status = AIEnum.Done,
                        Result = result
                    });

                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    _cache.Set(taskMessage.TaskId, new AITaskState
                    {
                        TaskId = taskMessage.TaskId,
                        Status = AIEnum.Failed,
                        Error = ex.Message
                    });

                    // khong retry de tiep tiem api ,co the them de nang cap
                    await channel.BasicAckAsync(ea.DeliveryTag, false);
                }
            };

            await channel.BasicConsumeAsync(
                queue: StringEnum.AI_Queue,
                autoAck: false,
                consumer: consumer);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }
}
