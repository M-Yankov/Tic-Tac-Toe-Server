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

        public int GamesWon { get; set; }

        public int GamesLose { get; set; }

        public int GamesDraw { get; set; }

        public int TotalGames { get; set; }
        
        public void CreateMappings(IConfiguration configuration)
        {
            configuration.CreateMap<User, UserResponseModel>()
                         .ForMember(x => x.Username, opts => opts.MapFrom(z => z.UserName))
                         .ForMember(x => x.TotalGames, opts => opts.MapFrom(u => u.CreatedGames.Count + u.JoinedGames.Count))
                         .ForMember(x => x.GamesDraw, 
                                    opts => opts.MapFrom(u => u.CreatedGames.Where(g => g.State == GameState.Draw).Count() +                                          u.JoinedGames.Where(g => g.State == GameState.Draw).Count()))
                         .ForMember(x => x.GamesWon,
                                    opts => opts.MapFrom(u => 
                                        u.CreatedGames.Where(g => g.FirstPlayerId == u.Id &&
                                                                    g.FirstPlayerSymbol == GameChar.O &&
                                                                    g.State == GameState.WonByO).Count() + 
                                        u.JoinedGames.Where(g => g.SecondPlayerId == u.Id && 
                                                                    g.SecondPlayerSymbol == GameChar.X &&
                                                                    g.State == GameState.WonByX).Count()))
                         .ForMember(x => x.GamesLose,
                                    opts => opts.MapFrom(u =>
                                    u.CreatedGames.Where(g => g.FirstPlayerId == u.Id &&
                                                                g.FirstPlayerSymbol == GameChar.O &&
                                                                g.State == GameState.WonByX).Count() +
                                    u.JoinedGames.Where(g => g.SecondPlayerId == u.Id &&
                                                                g.SecondPlayerSymbol == GameChar.X &&                          
                                                                g.State == GameState.WonByO).Count()));
        }
    }
}