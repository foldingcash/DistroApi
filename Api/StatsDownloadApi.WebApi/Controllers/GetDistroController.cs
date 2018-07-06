namespace StatsDownloadApi.WebApi.Controllers
{
    using CastleWindsor;
    using Core;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    public class GetDistroController : Controller
    {
        [HttpGet]
        public DistroResponse Get()
        {
            IStatsDownloadApiService apiService = null;
            try
            {
                apiService = WindsorContainer.Instance.Resolve<IStatsDownloadApiService>();
                DistroResponse response = apiService.GetDistro();
                return response;
            }
            finally
            {
                WindsorContainer.Instance.Release(apiService);
            }
        }
    }
}