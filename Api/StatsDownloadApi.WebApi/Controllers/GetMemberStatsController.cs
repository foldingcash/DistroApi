namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using StatsDownloadApi.Interfaces;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class GetMemberStatsController : ApiControllerBase
    {
        public GetMemberStatsController(ILogger<GetMemberStatsController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof (GetMemberStatsResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(DateTime? startDate, DateTime? endDate)
        {
            return await InvokeApiService(apiService => apiService.GetMemberStats(startDate, endDate));
        }
    }
}