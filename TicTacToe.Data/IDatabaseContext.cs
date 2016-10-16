namespace TicTacToe.Data
{
    using System.Data.Entity;
    using TicTacToe.Data.Models;

    public interface IDatabaseContext
    {
        IDbSet<Game> Games { get; set; }

        Database Database { get; }

        
    }
}
