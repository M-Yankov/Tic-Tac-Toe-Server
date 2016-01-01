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

        public Guid Add(string hostPlayerId)
        {
            var newGame = new Game { FirstPlayerId = hostPlayerId };
            this.dataSystem.Games.Add(newGame);
            this.dataSystem.Games.SaveChanges();
            return newGame.Id;
        }

        public Game GetFirstGameAvailableForJoin(string currentUserId)
        {
            Game game = this.dataSystem.Games
                .All()
                .Where(g => g.State == GameState.WaitingForSecondPlayer && g.FirstPlayerId != currentUserId)
                .FirstOrDefault();

            if (game == null)
            {
                return null;
            }

            // TODO: GameState Should be random !!!!! 
            game.SecondPlayerId = currentUserId;
            game.State = GameState.TurnX;

            this.dataSystem.SaveChanges();

            return game;
        }

        public Game GetGameDetails(Guid idOfTheGame)
        {
            return this.dataSystem.Games.GetById(idOfTheGame);
        }

        public IQueryable<Game> GetNewestGames(int countToTake)
        {
            return this.dataSystem.Games.All().Take(countToTake);
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

            this.dataSystem.SaveChanges();

            User firstUser = this.dataSystem.Users.GetById(game.FirstPlayerId);
            User secondUser = this.dataSystem.Users.GetById(game.SecondPlayerId);

            GameResult gameResult = this.gameResultValidator.GetResult(game.Board);

            // TODO try something else .
            switch (gameResult)
            {
                case GameResult.NotFinished:
                    break;
                case GameResult.WonByX:
                    game.State = GameState.WonByX;
                    this.dataSystem.SaveChanges();
                    firstUser.GamesWon++;
                    secondUser.GamesLose++;
                    this.dataSystem.SaveChanges();
                    break;
                case GameResult.WonByO:
                    game.State = GameState.WonByO;
                    this.dataSystem.SaveChanges();
                    firstUser.GamesLose++;
                    secondUser.GamesWon++;
                    this.dataSystem.SaveChanges();
                    break;
                case GameResult.Draw:
                    game.State = GameState.Draw;
                    this.dataSystem.SaveChanges();

                    firstUser.GamesDraw++;
                    secondUser.GamesDraw++;
                    this.dataSystem.SaveChanges();
                    break;
                default:
                    break;
            }
        }
    }
}