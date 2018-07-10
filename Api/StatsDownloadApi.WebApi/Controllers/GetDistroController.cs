namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using CastleWindsor;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class GetDistroController : Controller
    {
        [HttpGet]
        public DistroResponse Get(DateTime? startDate, DateTime? endDate)
        {
            IStatsDownloadApiService apiService = null;
            try
            {
                apiService = WindsorContainer.Instance.Resolve<IStatsDownloadApiService>();
                DistroResponse response = apiService.GetDistro(startDate, endDate);
                return response;
            }
            finally
            {
                WindsorContainer.Instance.Release(apiService);
            }
        }
    }
}