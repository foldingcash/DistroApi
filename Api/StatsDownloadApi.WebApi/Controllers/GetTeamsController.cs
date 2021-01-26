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
    public class GetTeamsController : ApiControllerBase
    {
        public GetTeamsController(ILogger<GetTeamsController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        /// <summary>
        ///     Get all teams
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof (GetTeamsResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get()
        {
            return await InvokeApiService(apiService => apiService.GetTeams());
        }
    }
}