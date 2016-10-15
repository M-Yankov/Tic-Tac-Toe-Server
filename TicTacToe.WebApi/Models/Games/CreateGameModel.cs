namespace TicTacToe.WebApi.Models.Games
{
    using System.ComponentModel.DataAnnotations;
    using Data.Models;

    public class CreateGameModel
    {
        [Required]
        [StringLength(DataModelConstants.MaxLengthName)]
        [MinLength(DataModelConstants.MinLengthName)]
        public string Name { get; set; }

        [Required]
        public GameChar StartChar { get; set; } 

        public bool IsPrivate { get; set; }

        public string Password { get; set; }
    }
}