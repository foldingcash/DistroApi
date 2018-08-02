namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using CastleWindsor;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [Produces("application/json")]
    [Route("v1/[controller]")]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(GetTeamsController))]
    public class GetTeamsController : Controller
    {
        [HttpGet]
        public GetTeamsResponse Get()
        {
            IStatsDownloadApiService apiService = null;
            try
            {
                apiService = WindsorContainer.Instance.Resolve<IStatsDownloadApiService>();
                GetTeamsResponse response = apiService.GetTeams();
                return response;
            }
            catch (Exception)
            {
                return new GetTeamsResponse(new[] { Constants.ApiErrors.UnexpectedException });
            }
            finally
            {
                WindsorContainer.Instance.Release(apiService);
            }
        }
    }
}