namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetDistroController))]
    public class GetDistroController : ApiControllerBase
    {
        [HttpGet]
        public ApiResponse Get(DateTime? startDate, DateTime? endDate, int? amount)
        {
            return InvokeApiService(apiService => apiService.GetDistro(startDate, endDate, amount));
        }

        [HttpGet("Next")]
        public ApiResponse GetNextDistro()
        {
            var startDate = new DateTime(2000, 10, 3);
            var endDate = DateTime.Today.AddDays(-1);
            var amount = 100000;
            return InvokeApiService(apiService => apiService.GetDistro(startDate, endDate, amount));
        }
    }
}