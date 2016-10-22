namespace TicTacToe.WebApi.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Constants;
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

            bool isGameExists = this.gameService.All().Any(g => g.Name.ToLower() == newGame.Name.ToLower());
            if (isGameExists)
            {
                return this.BadRequest("Game already exists! Try different name.");
            }

            string currentUserId = this.User.Identity.GetUserId();
            Guid idOfTheCreatedGame = this.gameService.Add(
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
        public IHttpActionResult Status(string gameId, bool isForJoin = false)
        {
            Guid idOfTheGame = Guid.Empty;

            if (!Guid.TryParse(gameId, out idOfTheGame))
            {
                return this.NotFound();
            }

            Game game = this.gameService.GetGameDetails(idOfTheGame);
            if (game == null)
            {
                return this.NotFound();
            }

            string currentUserId = this.User.Identity.GetUserId();
            if (game.FirstPlayerId != currentUserId &&
                game.SecondPlayerId != currentUserId)
            {
                if (!isForJoin)
                {
                    return this.BadRequest("This is not your game!");
                }
            }

            GameResponseModel gameInfo = Mapper.Map<GameResponseModel>(game);
            return Ok(gameInfo);
        }

        [HttpGet]
        [AllowAnonymous]
        public IHttpActionResult All(
            GameState state = GameState.Invalid,
            string playerName = "",
            string gameName = "",
            int count = WebApiConstants.DefaultCountOfGamesToShow)
        {
            gameName = gameName.ToLower();
            playerName = playerName.ToLower();

            IQueryable<Game> publicGames = this.gameService
                            .All()
                            .Where(g => g.IsPrivate == false &&
                                        g.Name.ToLower().Contains(gameName) &&
                                        (g.FirstPlayer.UserName.ToLower().Contains(playerName) ||
                                        (g.State != 0 && g.SecondPlayer.UserName.ToLower().Contains(playerName))));

            if (state != GameState.Invalid)
            {
                publicGames = publicGames.Where(g => g.State == state);
            }

            IEnumerable<GameResponseModel> responseGames = publicGames.OrderByDescending(g => g.DateCreated)
                                                                .Take(count)
                                                                .ProjectTo<GameResponseModel>().ToList();

            return this.Ok(responseGames);
        }

        [HttpGet]
        public IHttpActionResult PrivateGames()
        {
            string currentUserId = this.User.Identity.GetUserId();

            IEnumerable<GameResponseModel> gamesOfTheUser = this.gameService
                                                                .GetGamesByUserId(currentUserId)
                                                                .ProjectTo<GameResponseModel>()
                                                                .ToList();

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