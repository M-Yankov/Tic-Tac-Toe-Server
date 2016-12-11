namespace TicTacToe.WebApi.Models.Games
{
    using System;
    using AutoMapper;
    using TicTacToe.Data.Models;
    using TicTacToe.WebApi.Infrastructure;

    public class GameResponseModel : IMapFrom<Game>, IHaveCustomMappings
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Board { get; set; }

        public GameState State { get; set; }

        public bool IsPrivate { get; set; }

        public string DateCreated
        {
            get
            {
                return this.DateCreatedFull.ToString("dd.MM.yyyy");
            }
        }

        public DateTime DateCreatedFull { get; set; }

        public string FirstPlayerName { get; set; }

        public string SecondPlayerName { get; set; }

        public string FirstPlayerSymbol { get; set; }

        public string SecondPlayerSymbol { get; set; }

        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<Game, GameResponseModel>()
                    .ForMember(g => g.FirstPlayerName, opts => opts.MapFrom(x => x.FirstPlayer.UserName))
                    .ForMember(g => g.SecondPlayerName, opts => opts.MapFrom(x => x.SecondPlayer.UserName))
                    .ForMember(g => g.FirstPlayerSymbol, opts => opts.MapFrom(x => x.FirstPlayerSymbol.ToString()))
                    .ForMember(g => g.DateCreatedFull, opts => opts.MapFrom(x => x.DateCreated))
                    .ForMember(g => g.SecondPlayerSymbol, opts => opts.MapFrom(x => x.SecondPlayerSymbol.HasValue ?
                                                                                    x.SecondPlayerSymbol.Value.ToString() : string.Empty));
        }
    }
}