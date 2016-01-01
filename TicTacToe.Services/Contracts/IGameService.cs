namespace TicTacToe.Services.Contracts
{
    using System;
    using System.Linq;
    using TicTacToe.Data.Models;

    public interface IGameService
    {
        Guid Add(string hostPlayerId);

        Game GetFirstGameAvailableForJoin(string currentUserId);

        Game GetGameDetails(Guid idOfTheGame);

        IQueryable<Game> GetNewestGames(int countToTake);

        IQueryable<Game> GetGamesByUserId(string userId);

        void Play(Game game, int positionIndex);
    }
}
