namespace TicTacToe.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNet.Identity;
    using TicTacToe.Data.Models;
    using TicTacToe.Services.Contracts;
    using TicTacToe.WebApi.Models.Games;
    using TicTacToe.WebApi.Models.Users;

    [Authorize]
    public class GamesController : ApiController
    {
        private readonly IGameService gameService;
        private readonly IUserService userService;

        public GamesController(IGameService gameService, IUserService userService)
        {
            this.gameService = gameService;
            this.userService = userService;
        }

        [HttpPost]
        public IHttpActionResult Create()
        {
            var currentUserId = this.User.Identity.GetUserId();
            var idOfTheCreatedGame = this.gameService.Add(currentUserId);

            return Ok(idOfTheCreatedGame);
        }

        [HttpPost]
        public IHttpActionResult Join()
        {
            var currentUserId = this.User.Identity.GetUserId();

            Game game = this.gameService.GetFirstGameAvailableForJoin(currentUserId);

            if (game == null)
            {
                return NotFound();
            }

            return Ok(game.Id);
        }

        [HttpGet]
        public IHttpActionResult Status(string gameId)
        {
            string currentUserId = this.User.Identity.GetUserId();
            var idAsGuid = new Guid(gameId);

            Game game = this.gameService.GetGameDetails(idAsGuid);

            if (game == null)
            {
                return this.NotFound();
            }

            if (game.FirstPlayerId != currentUserId &&
                game.SecondPlayerId != currentUserId)
            {
                return this.BadRequest("This is not your game!");
            }

            GameResponseModel gameInfo = Mapper.Map<GameResponseModel>(game);
            return Ok(gameInfo);
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult All()
        {
            // TODO: better logic and sort by date descending i.e. newest.
            var topGames = this.gameService.GetNewestGames(10).ProjectTo<GameResponseModel>().ToList();
            return this.Ok(topGames);
        }

        [HttpGet]
        public IHttpActionResult PrivateGames()
        {
            var currentUserId = this.User.Identity.GetUserId();

            var gamesOfTheUser = this.gameService.GetGamesByUserId(currentUserId).ProjectTo<GameResponseModel>().ToList();

            return this.Ok(gamesOfTheUser);
        }

        [HttpGet]
        public IHttpActionResult Info()
        {
            var currentUserId = this.User.Identity.GetUserId();
            User user = this.userService.GetUserInfo(currentUserId);

            if (user == null)
            {
                return this.InternalServerError(new SystemException("User not found!"));
            }

            UserResponseModel responseUser = Mapper.Map<UserResponseModel>(user);

            return this.Ok(responseUser);
        }

        /// <param name="row">1,2 or 3</param>
        /// <param name="col">1,2 or 3</param>
        [HttpPost]
        public IHttpActionResult Play(PlayRequestDataModel request)
        {
            if (request == null || !ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var currentUserId = this.User.Identity.GetUserId();
            var idAsGuid = new Guid(request.GameId);
            var game = this.gameService.GetGameDetails(idAsGuid);

            if (game == null)
            {
                return this.BadRequest("Invalid game id!");
            }

            if (game.FirstPlayerId != currentUserId &&
                game.SecondPlayerId != currentUserId)
            {
                return this.BadRequest("This is not your game!");
            }

            if (game.State != GameState.TurnX &&
                game.State != GameState.TurnO)
            {
                return this.BadRequest("Invalid game state!");
            }

            if ((game.State == GameState.TurnX &&
                game.FirstPlayerId != currentUserId)
                ||
                (game.State == GameState.TurnO &&
                game.SecondPlayerId != currentUserId))
            {
                return this.BadRequest("It's not your turn!");
            }

            var positionIndex = (request.Row - 1) * 3 + request.Col - 1;
            if (game.Board[positionIndex] != '-')
            {
                return this.BadRequest("Invalid position!");
            }

            this.gameService.Play(game, positionIndex);

            return this.Ok();
        }
    }
}