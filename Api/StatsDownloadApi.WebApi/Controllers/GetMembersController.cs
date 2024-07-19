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
    public class GetMembersController : ApiControllerBase
    {
        public GetMembersController(ILogger<GetMembersController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        /// <summary>
        ///     Get all members
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof (GetMemberStatsResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            var fahStartDate = FoldingAtHome.FoldingAtHomeStartDate;
            DateTime today = DateTime.UtcNow.Date;
            return await InvokeApiService(apiService => apiService.GetMemberStats(fahStartDate, today));
        }
    }
}