namespace TicTacToe.WebApi
{
    using System.Data.Entity;
    using TicTacToe.Data;
    using TicTacToe.Data.Migrations;

    public class DatabaseConfig
    {
        public static void Initialize()
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DefaultDbContext, Configuration>());
        }
    }
}