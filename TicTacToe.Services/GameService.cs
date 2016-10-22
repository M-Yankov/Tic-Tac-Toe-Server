namespace TicTacToe.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TicTacToe.Data.Models;
    using TicTacToe.Data.Units;
    using TicTacToe.GameLogic;
    using TicTacToe.Services.Contracts;

    public class GameService : IGameService
    {
        private readonly IUnitSystem dataSystem;
        private readonly IGameResultValidator gameResultValidator;

        public GameService(IUnitSystem system, IGameResultValidator resultValidator)
        {
            this.dataSystem = system;
            this.gameResultValidator = resultValidator;
        }

        public Guid Add(string hostPlayerId, string name, bool isPrivate, string password, GameChar startChar)
        {
            var newGame = new Game
            {
                FirstPlayerId = hostPlayerId,
                IsPrivate = isPrivate,
                Name = name,
                Password = password,
                DateCreated = DateTime.Now,
                FirstPlayerSymbol = startChar
            };

            this.dataSystem.Games.Add(newGame);
            this.dataSystem.Games.SaveChanges();
            return newGame.Id;
        }

        public JoinResultModel JoinGame(string currentUserId, string gameId, string password)
        {
            JoinResultModel result = new JoinResultModel();
           
            Game game = this.dataSystem.Games
                .All()
                .FirstOrDefault(g =>
                    g.Id.ToString() == gameId &&
                    g.State == GameState.WaitingForSecondPlayer &&
                    g.FirstPlayerId != currentUserId);

            if (game == null)
            {
                return null;
            }

            if (game.IsPrivate && game.Password != password)
            {
                result.IsSuccess = false;
                result.ErrroMessage = "Wrong password";
                return result;
            }
 
            game.SecondPlayerId = currentUserId;

            if (game.FirstPlayerSymbol == GameChar.O)
            {
                game.State = GameState.TurnO;
                game.SecondPlayerSymbol = GameChar.X;
            }
            else
            {
                game.State = GameState.TurnX;
                game.SecondPlayerSymbol = GameChar.O;
            }

            this.dataSystem.SaveChanges();

            result.IsSuccess = true;
            result.GameId = game.Id;
            return result;
        }

        public Game GetGameDetails(Guid idOfTheGame)
        {
            return this.dataSystem.Games.GetById(idOfTheGame);
        }

        public IQueryable<Game> All()
        {
            return this.dataSystem.Games.All();
        }

        public IQueryable<Game> GetGamesByUserId(string userId)
        {
            return this.dataSystem.Games
                 .All()
                 .Where(g => g.FirstPlayerId == userId || g.SecondPlayerId == userId);
        }

        public void Play(Game game, int positionIndex)
        {
            /// IF changes are not applicable to get the game with ID;
            // Update games state and board
            var boardAsStringBuilder = new StringBuilder(game.Board);
            boardAsStringBuilder[positionIndex] = game.State == GameState.TurnX ? 'X' : 'O';
            game.Board = boardAsStringBuilder.ToString();

            game.State = game.State == GameState.TurnX ? GameState.TurnO : GameState.TurnX;

            GameResult gameResult = this.gameResultValidator.GetResult(game.Board);

            switch (gameResult)
            {
                case GameResult.NotFinished:
                    break;
                case GameResult.WonByX:
                    game.State = GameState.WonByX;
                    game.WonById = game.FirstPlayerSymbol == GameChar.X ? game.FirstPlayerId : game.SecondPlayerId;
                    break;
                case GameResult.WonByO:
                    game.WonById = game.FirstPlayerSymbol == GameChar.O ? game.FirstPlayerId : game.SecondPlayerId;
                    game.State = GameState.WonByO;
                    break;
                case GameResult.Draw:
                    game.State = GameState.Draw;
                    break;
                default:
                    break;
            }

            this.dataSystem.SaveChanges();
        }
    }
}