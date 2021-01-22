namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetDistroController))]
    public class GetDistroController : ApiControllerBase
    {
        public GetDistroController(ILogger<GetDistroController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(DateTime? startDate, DateTime? endDate, int? amount,
                                             CancellationToken cancellationToken = default)
        {
            return await InvokeApiService(async apiService => await apiService.GetDistro(startDate, endDate, amount));
        }

        [HttpGet("All")]
        public IActionResult GetAllDistro(CancellationToken cancellationToken = default)
        {
            var startDate = new DateTime(2000, 10, 3);
            DateTime endDate = DateTime.Today.AddDays(-1);
            var amount = 100000;
            return RedirectToAction(nameof(Get), new { startDate, endDate, amount });
        }
    }
}