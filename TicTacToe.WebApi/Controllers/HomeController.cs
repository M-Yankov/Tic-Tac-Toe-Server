namespace TicTacToe.WebApi.Controllers
{
    using System.Linq;
    using System.Web;
    using System.Web.Http;
    using Services.Contracts;
    using Constants;

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
            return this.Ok(WebApiConstants.WelcomeMessage);
        }
    }
}
