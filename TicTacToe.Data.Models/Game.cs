namespace TicTacToe.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Game
    {
        public Game()
        {
            this.Id = Guid.NewGuid();
            this.Board = DataModelConstants.DefaultBoardContent;
            this.State = GameState.WaitingForSecondPlayer;
        }

        public Guid Id { get; set; }

        public DateTime DateCreated { get; set; }

        [Required]
        [StringLength(DataModelConstants.MaxLengthName, MinimumLength = DataModelConstants.MinLengthName)]
        public string Name { get; set; }

        public bool IsPrivate { get; set; }

        [StringLength(DataModelConstants.MaxLengthName)]
        public string Password { get; set; }

        [Column(TypeName = DataModelConstants.CharType)]
        [StringLength(DataModelConstants.BoardContentMaxLength, MinimumLength = DataModelConstants.BoardContentMaxLength)]
        public string Board { get; set; }

        public GameState State { get; set; }

        [Required]
        [StringLength(DataModelConstants.MaxLengthName)]
        public string FirstPlayerId { get; set; }

        public virtual User FirstPlayer { get; set; }

        public GameChar FirstPlayerSymbol { get; set; }

        [StringLength(DataModelConstants.MaxLengthName)]
        public string SecondPlayerId { get; set; }

        public virtual User SecondPlayer { get; set; }

        public GameChar? SecondPlayerSymbol { get; set; }
    }
}
