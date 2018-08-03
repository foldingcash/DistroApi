namespace StatsDownloadApi.WebApi.Controllers
{
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetMemberStatsController))]
    public class GetMemberStatsController : ApiControllerBase
    {
        [HttpGet]
        public GetMemberStatsResponse Get()
        {
            return InvokeApiService(apiService => apiService.GetMemberStats());
        }
    }
}