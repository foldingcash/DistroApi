namespace StatsDownloadApi.WebApi.Controllers
{
    using System;

    using Microsoft.AspNetCore.Mvc;

    using StatsDownloadApi.Interfaces;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetMembersController))]
    public class GetMembersController : ApiControllerBase
    {
        [HttpGet]
        public ApiResponse Get()
        {
            var fahStartDate = new DateTime(2000, 10, 3);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            return InvokeApiService(apiService => apiService.GetMemberStats(fahStartDate, yesterday));
        }
    }
}