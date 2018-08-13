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
        public GetDistroResponse Get(DateTime? startDate, DateTime? endDate, int? amount)
        {
            return InvokeApiService(apiService => apiService.GetDistro(startDate, endDate, amount));
        }
    }
}