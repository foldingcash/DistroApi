namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetMembersController))]
    public class GetMembersController : ApiControllerBase
    {
        public GetMembersController(ILogger<GetMembersController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var fahStartDate = new DateTime(2000, 10, 3);
            DateTime yesterday = DateTime.Today.AddDays(-1);
            return await InvokeApiService(apiService => apiService.GetMemberStats(fahStartDate, yesterday));
        }
    }
}