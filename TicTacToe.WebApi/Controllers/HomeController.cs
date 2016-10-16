namespace TicTacToe.WebApi.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            this.gameService.GetNewestGames(1);
            TimeSpan elapsed = stopWatch.Elapsed;

            stopWatch.Stop();
            string result = string.Format("{0}:{1}", elapsed.TotalMinutes, elapsed.TotalSeconds);

            return this.Ok(result);
        }
    }
}
