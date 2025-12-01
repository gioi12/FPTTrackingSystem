using DataTranferObjects.Common.Request;
using DataTranferObjects.Enum;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Services.Common.MQ;
using Quartz;

namespace FPTTrackingSystem.Services.Common.Schedules
{
    public class SendMailJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMQProducer _rabbitMQProducer;
        public SendMailJob(IServiceProvider serviceProvider,RabbitMQProducer rabbitMQProducer)
        {
            _serviceProvider = serviceProvider;
            _rabbitMQProducer=rabbitMQProducer;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            using var scope = _serviceProvider.CreateScope();
            var mailService = scope.ServiceProvider.GetRequiredService<IMailService>();

            await _rabbitMQProducer.SendMessage<List<MailRequest>>(new List<MailRequest>
            { 
                new MailRequest
                {
                    To = ["doangioi0403@gmail.com"],
                    Subject = "Báo cáo hàng ngày",
                    Body = "Đây là mail gửi tự động vào 7h sáng.",
                    Cc = []
                }
            },StringEnum.Mail_Queue);
        }
}
    }
