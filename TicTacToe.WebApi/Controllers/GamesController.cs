namespace TicTacToe.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web;
    using System.Web.Http;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNet.Identity;
    using Services;
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
        public IHttpActionResult Create(CreateGameModel newGame)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            if (newGame.IsPrivate && string.IsNullOrEmpty(newGame.Password))
            {
                return this.BadRequest("Missing password!");
            }

            var currentUserId = this.User.Identity.GetUserId();
            var idOfTheCreatedGame = this.gameService.Add(
                currentUserId,
                newGame.Name,
                newGame.IsPrivate,
                newGame.Password,
                newGame.StartChar);

            return Ok(idOfTheCreatedGame);
        }

        [HttpPost]
        public IHttpActionResult Join(string id, JoinModel model)
        {
            var currentUserId = this.User.Identity.GetUserId();

            JoinResultModel joinResult = this.gameService.JoinGame(currentUserId, id, model.Password);

            if (joinResult == null)
            {
                var response = new HttpResponseMessage(HttpStatusCode.NotFound);
                response.Content = new StringContent("Game not found!");
                return this.ResponseMessage(response);
            }

            if (!joinResult.IsSuccess)
            {
                return this.BadRequest(joinResult.ErrroMessage);
            }

            return Ok(joinResult.GameId);

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

        [HttpPost]
        public IHttpActionResult Play(PlayRequestDataModel request)
        {
            if (request == null || !this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
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

            GameChar nextCharTurn = game.State == GameState.TurnX ? GameChar.X : GameChar.O;
            string nextTurnPlayerId = game.FirstPlayerSymbol == nextCharTurn ? game.FirstPlayerId : game.SecondPlayerId;

            if ((game.FirstPlayerId == currentUserId && game.FirstPlayerSymbol != nextCharTurn) ||
                (game.SecondPlayerId == currentUserId && game.SecondPlayerSymbol != nextCharTurn))
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