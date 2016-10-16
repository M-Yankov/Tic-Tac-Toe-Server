namespace TicTacToe.WebApi.Controllers
{
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Services.Contracts;

    public class HomeController : ApiController
    {
        private IGameService gameService;

        public HomeController(IGameService dataGameService)
        {
            this.gameService = dataGameService;
        }

        [HttpGet]
        public IHttpActionResult Index()
        {
            this.gameService.GetNewestGames(1).ToList();

            object elapsed = HttpContext.Current.Application["elapsed"];
            string response = "Welcome";

            if (elapsed != null)
            {
                response = elapsed.ToString();
            }

            return this.Ok(response);
        }
    }
}
