namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using StatsDownloadApi.Interfaces;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class GetDistroController : ApiControllerBase
    {
        public GetDistroController(ILogger<GetDistroController> logger, IServiceProvider serviceProvider)
            : base(logger, serviceProvider)
        {
        }

        /// <summary>
        ///     Get a distribution based on a start and end date and proportionally split the amount
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="amount"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof (GetDistroResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(DateTime? startDate, DateTime? endDate, int? amount,
                                             CancellationToken cancellationToken = default)
        {
            return await InvokeApiService(async apiService => await apiService.GetDistro(startDate, endDate, amount));
        }

        /// <summary>
        ///     Get a system defined distribution
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("All")]
        [ProducesResponseType((int)HttpStatusCode.Redirect)]
        public IActionResult GetAllDistro(CancellationToken cancellationToken = default)
        {
            var startDate = new DateTime(2000, 10, 3);
            DateTime endDate = DateTime.Today.AddDays(-1);
            var amount = 100000;
            return RedirectToAction(nameof(Get), new { startDate, endDate, amount });
        }
    }
}