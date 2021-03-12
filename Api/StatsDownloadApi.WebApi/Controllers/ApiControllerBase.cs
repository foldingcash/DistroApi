namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using StatsDownloadApi.Interfaces;

    public abstract class ApiControllerBase : Controller
    {
        private readonly ILogger logger;

        private readonly IServiceProvider serviceProvider;

        protected ApiControllerBase(ILogger logger, IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
        }

        protected async Task<IActionResult> InvokeApiService<T>(
            Func<IStatsDownloadApiService, Task<T>> invokeApiServiceFunc)
            where T : ApiResponse
        {
            try
            {
                if (invokeApiServiceFunc == null)
                {
                    throw new ArgumentNullException(nameof(invokeApiServiceFunc));
                }

                var apiService = serviceProvider.GetRequiredService<IStatsDownloadApiService>();
                T result = await invokeApiServiceFunc.Invoke(apiService);

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
                logger.LogError(exception, "There was an unhandled exception");
                TrySendingUnhandledExceptionEmail(exception);
                return new ServerErrorObjectResult(new ApiResponse(new[] { Constants.ApiErrors.UnexpectedException }));
            }
        }

        private void TrySendingUnhandledExceptionEmail(Exception exception)
        {
            var emailService = serviceProvider.GetService<IStatsDownloadApiEmailService>();
            emailService?.SendUnhandledExceptionEmail(exception);
        }
    }
}