namespace StatsDownloadApi.WebApi.Controllers
{
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetTeamsController))]
    public class GetTeamsController : ApiControllerBase
    {
        [HttpGet]
        public ApiResponse Get()
        {
            return InvokeApiService(apiService => apiService.GetTeams());
        }
    }
}