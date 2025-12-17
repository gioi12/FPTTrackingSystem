using DataTranferObjects.Common.Request;
using DataTranferObjects.Common.Response;
using DataTranferObjects.Enum;
using Entities.Models;
using FPTTrackingSystem.Services.Common.MQ;
using Microsoft.Extensions.Caching.Memory;
using Repositories.Common.Interfaces;
using Repositories.Student.Interfaces;
using System.Threading.Tasks;

namespace FPTTrackingSystem.Services.Admin
{
    public class AIService : IAIService
    {
        private readonly IAISettingsRepository _repo;
        private readonly RabbitMQProducer _rabbitMQProducer;
        private readonly IMeetingRepository _meetingRepo;
        private readonly IMemoryCache _cache;
        public AIService(IAISettingsRepository repo,RabbitMQProducer rabbitMQProducer,IMeetingRepository meetingRepository,IMemoryCache cache)
        {
            _repo = repo;
            _rabbitMQProducer = rabbitMQProducer;
            _meetingRepo = meetingRepository;
            _cache = cache;
        }

        public async Task<string> AskAsync(string prompt,int? groupId)
        {
            var taskId = Guid.NewGuid().ToString();

            if(groupId != null)
            {
                prompt += await _meetingRepo.MeetingMinuteData((int)groupId);
            }

            var message = new AITaskMessage
            {
                TaskId = taskId,
                Prompt = prompt
            };
            _cache.Set(taskId, new AITaskState
            {
                TaskId = taskId,
                Status = AIEnum.Pending,
                CreatedAt = DateTime.UtcNow
            }, TimeSpan.FromMinutes(10));
            await _rabbitMQProducer.SendMessage(message, StringEnum.AI_Queue);
            return taskId;
        }

        public async Task<AISettingsRes> GetAISettings()
        {
            var aiSettings = await _repo.GetSettings();
            if (aiSettings == null)
            {
                return null;
            }
            var response = new AISettingsRes
            {
                Id = aiSettings.Id,
                Name = aiSettings.Name,
                IsActive = aiSettings.IsActive
            };
            return response;
        }

        public async Task<AISettingsRes> NewAISettings(NewAISettingsReq setting)
        {
            var newSetting = new Entities.Models.Aisetting
            {
                Name = setting.Name,
                SecretKey = setting.SecretKey,
                IsActive = true
            };
            var result = await _repo.NewSettings(newSetting);
            if (!result)
            {
                throw new Exception("Create new AI settings failed");
            }
            var response = new AISettingsRes
            {
                Id = newSetting.Id,
                Name = newSetting.Name,
                IsActive = newSetting.IsActive
            };
            return response;
        }
    }
}
