namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using StatsDownloadApi.Interfaces;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetMembersController))]
    public class GetMembersController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ApiResponse> Get()
        {
            var fahStartDate = new DateTime(2000, 10, 3);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            return await InvokeApiService(apiService => apiService.GetMemberStats(fahStartDate, yesterday));
        }
    }
}