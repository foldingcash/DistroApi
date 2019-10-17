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
            DateTime today = DateTime.Today;
            return InvokeApiService(apiService => apiService.GetMemberStats(today.AddDays(-2), today));
        }
    }
}