namespace TicTacToe.Services.Contracts
{
    using System;
    using System.Linq;
    using TicTacToe.Data.Models;

    public interface IGameService
    {
        Guid Add(string hostPlayerId, string name, bool isPrivate, string password, GameChar startChar);

        JoinResultModel JoinGame(string currentUserId, string gameId, string password);

        Game GetGameDetails(Guid idOfTheGame);

        IQueryable<Game> All();

        IQueryable<Game> GetGamesByUserId(string userId);

        void Play(Game game, int positionIndex);
    }
}
