using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVCUseDotnetCoreDI.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<HomeController> logger;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            this.httpClient = httpClientFactory.CreateClient();
            this.logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var result = await httpClient.GetStringAsync("https://www.mocky.io/v2/5185415ba171ea3a00704eed");
            logger.LogDebug(result);

            ViewBag.Message = result;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}