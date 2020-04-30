using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Werewolf.Models
{
    public class Game
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public int Turn { get; set; }

        [Required]
        public DateTime TurnStarted { get; set; }

        [Required]
        public int PlayerCount { get; set; }
    }
}