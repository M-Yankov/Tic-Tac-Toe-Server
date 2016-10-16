namespace TicTacToe.Services
{
    using System;

    public class JoinResultModel
    {
        public bool IsSuccess { get; set; }

        public string ErrroMessage { get; set; }

        public Guid GameId { get; set; }
    }
}
