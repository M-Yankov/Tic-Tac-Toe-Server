namespace TicTacToe.WebApi.Models.Users
{
    using System;
    using System.Linq;
    using AutoMapper;
    using TicTacToe.Data.Models;
    using TicTacToe.WebApi.Infrastructure;

    public class UserResponseModel : IMapFrom<User>, IHaveCustomMappings
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string GamesWon { get; set; }

        public string GamesLose { get; set; }

        public string GamesDraw { get; set; }

        public string TotalGames { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<User, UserResponseModel>()
                         .ForMember(x => x.Username, opts => opts.MapFrom(z => z.UserName))
                         .ForMember(x => x.TotalGames, opts => opts.MapFrom(u => u.GamesWon + u.GamesLose + u.GamesDraw));
        }
    }
}