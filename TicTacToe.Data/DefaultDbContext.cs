namespace TicTacToe.Data
{
    using System;
    using System.Data.Entity;
    using TicTacToe.Data.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.ModelConfiguration;

    public class DefaultDbContext : IdentityDbContext<User>, IDatabaseContext
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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>()
                .HasRequired(g => g.FirstPlayer)
                .WithMany(u => u.CreatedGames)
                .HasForeignKey(u => u.FirstPlayerId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Game>()
                .HasOptional(g => g.SecondPlayer)
                .WithMany(u => u.JoinedGames)
                .HasForeignKey(u => u.SecondPlayerId)
                .WillCascadeOnDelete(false);
        }
    }
}
