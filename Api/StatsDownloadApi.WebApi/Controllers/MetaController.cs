namespace StatsDownloadApi.WebApi.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [Produces("application/json")]
    [Route("[controller]")]
    [Route("v1/[controller]")]
    public class MetaController : ApiControllerBase
    {
        public MetaController(ILogger<MetaController> logger, IServiceProvider serviceProvider) : base(logger,
            serviceProvider)
        {
        }

        /// <summary>
        ///     Get a message to sign and verify ownership
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet("{name}")]
        public Task<IActionResult> GetMessage([FromRoute] string name, CancellationToken cancellationToken = default)
        {
            return null;
        }

        /// <summary>
        ///     Update the user's meta data
        /// </summary>
        /// <param name="name"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost("{name}")]
        public Task<IActionResult> UpdateMeta([FromRoute] string name, [FromBody] MetaUpdateRequest request,
            CancellationToken cancellationToken = default)
        {
            return null;
        }
    }

    public class MetaUpdateRequest
    {
        public string Address { get; set; }

        public string Signature { get; set; }

        public MetaAddressType Type { get; set; }
    }

    public enum MetaAddressType
    {
        CashTokens
    }
}