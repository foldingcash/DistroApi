namespace StatsDownloadApi.WebApi.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetTeamsController))]
    public class GetTeamsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await InvokeApiService(apiService => apiService.GetTeams());
        }
    }
}