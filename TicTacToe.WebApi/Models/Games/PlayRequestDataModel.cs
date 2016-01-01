namespace TicTacToe.WebApi.Models.Games
{
    using System.ComponentModel.DataAnnotations;

    public class PlayRequestDataModel
    {
        [Required]
        public string GameId { get; set; }

        [Required]
        [Range(1, 3)]
        public int Row { get; set; }

        [Required]
        [Range(1, 3)]
        public int Col { get; set; }
    }
}