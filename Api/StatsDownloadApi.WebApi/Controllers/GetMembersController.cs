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
        [HttpGet("All")]
        [ProducesResponseType(typeof (GetMembersResponse), (int)HttpStatusCode.OK)]
        public IActionResult GetAll()
        {
            var fahStartDate = FoldingAtHome.FoldingAtHomeStartDate;
            DateTime today = DateTime.UtcNow.Date;
            return RedirectToAction(nameof(Get), new { startDate = fahStartDate, endDate = today });
        }

        /// <summary>
        ///     Get members based on a start and end date
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(GetMembersResponse), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(DateTime? startDate, DateTime? endDate)
        {
            return await InvokeApiService(apiService => apiService.GetMembers(startDate, endDate));
        }
    }
}