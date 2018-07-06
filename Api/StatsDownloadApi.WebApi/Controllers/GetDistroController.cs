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
            IStatsDownloadApi api = null;
            try
            {
                api = WindsorContainer.Instance.Resolve<IStatsDownloadApi>();
                DistroResponse response = api.GetDistro();
                return response;
            }
            finally
            {
                WindsorContainer.Instance.Release(api);
            }
        }
    }
}