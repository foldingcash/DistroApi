namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using CastleWindsor;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetDistroController))]
    public class GetDistroController : Controller
    {
        [HttpGet]
        public GetDistroResponse Get(DateTime? startDate, DateTime? endDate, int? amount)
        {
            IStatsDownloadApiService apiService = null;
            try
            {
                apiService = WindsorContainer.Instance.Resolve<IStatsDownloadApiService>();
                GetDistroResponse response = apiService.GetDistro(startDate, endDate, amount);
                return response;
            }
            catch (Exception)
            {
                return new GetDistroResponse(new[] { Constants.ApiErrors.UnexpectedException });
            }
            finally
            {
                WindsorContainer.Instance.Release(apiService);
            }
        }
    }
}