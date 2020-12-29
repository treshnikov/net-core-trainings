using System;
using System.Globalization;
using System.Security.Claims;
using BusinessLogic.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JoggingWebApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeekReportController : ControllerBase
    {
        private readonly IWeekReportProvider _weekReportProvider;
        
        private long CurrentUserId => Convert.ToInt64(User.FindFirstValue(ClaimTypes.Name));

        public WeekReportController(IWeekReportProvider weekReportProvider)
        {
            _weekReportProvider = weekReportProvider;
        }
        
        [HttpGet("{start:datetime}/{end:datetime}")]
        [HasPermission(Permissions.ManageOwnJoggingData)]
        public IActionResult Get(DateTime start, DateTime end, int? skip = null, int? take = null)
        {
            if (skip < 0 || take < 1)
            {
                return BadRequest();
            }
            
            var result = _weekReportProvider.GetReport(CurrentUserId, start, end, skip, take);
            return Ok(result);
        }
        
        [HttpGet("{userId}/{start:datetime}/{end:datetime}")]
        [HasPermission(Permissions.ManageOthersJoggingData)]
        public IActionResult Get(long userId, DateTime start, DateTime end, int? skip = null, int? take = null)
        {
            if (userId <= 0)
            {
                return BadRequest();
            }
            
            if (skip < 0 || take < 1)
            {
                return BadRequest();
            }
            
            var result = _weekReportProvider.GetReport(userId, start, end, skip, take);
            return Ok(result);
        }
    }
}