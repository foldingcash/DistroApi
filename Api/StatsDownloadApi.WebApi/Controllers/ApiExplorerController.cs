namespace StatsDownloadApi.WebApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;

    [Route("v1")]
    [Route("")]
    public class ApiExplorerController : Controller
    {
        private readonly IApiDescriptionGroupCollectionProvider apiExplorer;

        public ApiExplorerController(IApiDescriptionGroupCollectionProvider apiExplorer)
        {
            this.apiExplorer = apiExplorer;
        }

        public IActionResult Index()
        {
            return View(apiExplorer);
        }
    }
}