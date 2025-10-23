using DataTranferObjects.Common.Request;
using FPTTrackingSystem.Services.Common.Interfaces;
using FPTTrackingSystem.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FPTTrackingSystem.Controllers.Common
{
    [Route("api/")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly IMailService _mailService;
        public MailController(IMailService mailService)
        {
            _mailService = mailService;
        }
        [Authorize]
        [HttpPost("v1/Mail/send-mails")]
        public async Task<object> SendMail([FromBody] MailRequest request)
        {
            await _mailService.SendAnnounceMail(request);
            return Ok(ApiResponse<object>.Success(null, "Send mail successfully."));
        }
    }
}
