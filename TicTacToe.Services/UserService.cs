namespace TicTacToe.Services
{
    using System.Linq;
    using TicTacToe.Data.Models;
    using TicTacToe.Data.Units;
    using TicTacToe.Services.Contracts;

    public class UserService : IUserService
    {
        private IUnitSystem dataSystem;

        public UserService(IUnitSystem system)
        {
            this.dataSystem = system;
        }

        public User GetUserInfo(string id)
        {
            return this.dataSystem.Users.GetById(id);
        }
    }
}
