namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using StatsDownload.Core.Interfaces.Logging;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.WebApi.CastleWindsor;

    public abstract class ApiControllerBase : Controller
    {
        protected async Task<IActionResult> InvokeApiService<T>(
            Func<IStatsDownloadApiService, Task<T>> invokeApiServiceFunc)
            where T : ApiResponse
        {
            IStatsDownloadApiService apiService = null;
            try
            {
                apiService = WindsorContainer.Instance.Resolve<IStatsDownloadApiService>();
                T result = await invokeApiServiceFunc?.Invoke(apiService);

                if (result.Success)
                {
                    return new OkObjectResult(result);
                }

                bool isMinorIssue = result.Errors.Any(error => error.ErrorCode <= ApiErrorCode.MinorIssuesMax);

                if (isMinorIssue)
                {
                    return new BadRequestObjectResult(result);
                }

                return new ServerErrorObjectResult(result);
            }
            catch (Exception exception)
            {
                TryLoggingUnhandledException(exception);
                TrySendingUnhandledExceptionEmail(exception);
                return new ServerErrorObjectResult(new ApiResponse(new[] { Constants.ApiErrors.UnexpectedException }));
            }
            finally
            {
                WindsorContainer.Instance.Release(apiService);
            }
        }

        private void TryLoggingUnhandledException(Exception exception)
        {
            ILoggingService loggingService = null;
            try
            {
                loggingService = WindsorContainer.Instance.Resolve<ILoggingService>();
                loggingService.LogException(exception);
            }
            finally
            {
                WindsorContainer.Instance.Release(loggingService);
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