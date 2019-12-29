namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using StatsDownloadApi.Interfaces;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetMemberStatsController))]
    public class GetMemberStatsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ApiResponse> Get(DateTime? startDate, DateTime? endDate)
        {
            return await InvokeApiService(apiService => apiService.GetMemberStats(startDate, endDate));
        }
    }
}