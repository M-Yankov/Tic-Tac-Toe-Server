﻿namespace TicTacToe.Data.Models
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class User : IdentityUser
    {
        public virtual ICollection<Game> CreatedGames { get; set; }
        public virtual ICollection<Game> JoinedGames { get; set; }

        //public int GamesWon { get; set; }

        //public int GamesLose { get; set; }

        //public int GamesDraw { get; set; }

        //public int Age { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }
    }
}
