namespace TicTacToe.Services.Contracts
{
    using TicTacToe.Data.Models;

    public interface IUserService
    {
        User GetUserInfo(string id);
    }
}
