namespace StatsDownloadApi.WebApi.Controllers
{
    using CastleWindsor;
    using Core;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/GetDistro")]
    public class DistroController : Controller
    {
        public DistroResponse Get()
        {
            var api = WindsorContainer.Instance.Resolve<IStatsDownloadApi>();
            var response = api.GetDistro();
            WindsorContainer.Instance.Release(api);
            return response;
        }
    }
}