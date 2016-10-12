namespace TicTacToe.WebApi.Controllers
{
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
            this.gameService.GetNewestGames(1);
            return this.Ok("Welcome");
        }
    }
}
