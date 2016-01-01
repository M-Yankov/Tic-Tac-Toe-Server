namespace TicTacToe.WebApi.Models.Games
{
    using System;
    using AutoMapper;
    using TicTacToe.Data.Models;
    using TicTacToe.WebApi.Infrastructure;

    public class GameResponseModel : IMapFrom<Game>, IHaveCustomMappings
    {
        public Guid Id { get; set; }

        public string Board { get; set; }

        public GameState State { get; set; }

        public string FirstPlayerName { get; set; }

        public string SecondPlayerName { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Game, GameResponseModel>()
                    .ForMember(g => g.FirstPlayerName, opts => opts.MapFrom(x => x.FirstPlayer.UserName))
                    .ForMember(g => g.SecondPlayerName, opts => opts.MapFrom(x => x.SecondPlayer.UserName));
        }
    }
}