namespace TicTacToe.Data.Units
{
    using System;
    using TicTacToe.Data.Models;
    using TicTacToe.Data.Repositories;

    public interface IUnitSystem
    {
        IRepository<User> Users { get; }

        IRepository<Game> Games { get; }

        int SaveChanges();
    }
}
