namespace TicTacToe.Data
{
    using System;
    using System.Data.Entity;
    using TicTacToe.Data.Models;
    using Microsoft.AspNet.Identity.EntityFramework;

    public class DefaultDbContext : IdentityDbContext<User>
    {
        public DefaultDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<Game> Games { get; set; }

        public static DefaultDbContext Create()
        {
            return new DefaultDbContext();
        }
    }
}
