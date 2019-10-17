namespace StatsDownloadApi.WebApi.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    using StatsDownloadApi.Interfaces;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetMemberStatsController))]
    public class GetMemberStatsController : ApiControllerBase
    {
        [HttpGet]
        public ApiResponse Get(DateTime? startDate, DateTime? endDate)
        {
            return InvokeApiService(apiService => apiService.GetMemberStats(startDate, endDate));
        }
    }
}