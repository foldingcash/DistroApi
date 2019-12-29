namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using StatsDownloadApi.Interfaces;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetDistroController))]
    public class GetDistroController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ApiResponse> Get(DateTime? startDate, DateTime? endDate, int? amount)
        {
            return await InvokeApiService(apiService => apiService.GetDistro(startDate, endDate, amount));
        }

        [HttpGet("Next")]
        public async Task<ApiResponse> GetNextDistro()
        {
            var startDate = new DateTime(2000, 10, 3);
            DateTime endDate = DateTime.Today.AddDays(-1);
            var amount = 100000;
            return await InvokeApiService(apiService => apiService.GetDistro(startDate, endDate, amount));
        }
    }
}