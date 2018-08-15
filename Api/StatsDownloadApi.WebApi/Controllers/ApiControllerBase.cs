namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using CastleWindsor;
    using Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public abstract class ApiControllerBase : Controller
    {
        protected ApiResponse InvokeApiService<T>(Func<IStatsDownloadApiService, T> invokeApiServiceFunc)
            where T : ApiResponse
        {
            IStatsDownloadApiService apiService = null;
            try
            {
                apiService = WindsorContainer.Instance.Resolve<IStatsDownloadApiService>();
                return invokeApiServiceFunc?.Invoke(apiService);
            }
            catch (Exception exception)
            {
                TrySendingUnhandledExceptionEmail(exception);
                return new ApiResponse(new[] { Constants.ApiErrors.UnexpectedException });
            }
            finally
            {
                WindsorContainer.Instance.Release(apiService);
            }
        }

        private void TrySendingUnhandledExceptionEmail(Exception exception)
        {
            IStatsDownloadApiEmailService emailService = null;
            try
            {
                emailService = WindsorContainer.Instance.Resolve<IStatsDownloadApiEmailService>();
                emailService.SendUnhandledExceptionEmail(exception);
            }
            finally
            {
                WindsorContainer.Instance.Release(emailService);
            }
        }
    }
}