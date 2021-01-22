namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class GetTeamsController : ApiControllerBase
    {
        public GetTeamsController(ILogger<GetTeamsController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await InvokeApiService(apiService => apiService.GetTeams());
        }
    }
}