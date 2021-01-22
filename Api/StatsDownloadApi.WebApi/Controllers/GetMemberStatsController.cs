namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetMemberStatsController))]
    public class GetMemberStatsController : ApiControllerBase
    {
        public GetMemberStatsController(ILogger<GetMemberStatsController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? startDate, DateTime? endDate)
        {
            return await InvokeApiService(apiService => apiService.GetMemberStats(startDate, endDate));
        }
    }
}