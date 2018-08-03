namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using CastleWindsor;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public abstract class ApiControllerBase : Controller
    {
        protected T InvokeApiService<T>(Func<IStatsDownloadApiService, T> invokeApiServiceFunc) where T : ApiResponse
        {
            IStatsDownloadApiService apiService = null;
            try
            {
                apiService = WindsorContainer.Instance.Resolve<IStatsDownloadApiService>();
                return invokeApiServiceFunc?.Invoke(apiService);
            }
            catch (Exception)
            {
                return new ApiResponse(new[] { Constants.ApiErrors.UnexpectedException }) as T;
            }
            finally
            {
                WindsorContainer.Instance.Release(apiService);
            }
        }
    }
}