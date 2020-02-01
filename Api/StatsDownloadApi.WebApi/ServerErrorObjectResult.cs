namespace StatsDownloadApi.WebApi
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class ServerErrorObjectResult : ObjectResult
    {
        public ServerErrorObjectResult(object value)
            : base(value)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}