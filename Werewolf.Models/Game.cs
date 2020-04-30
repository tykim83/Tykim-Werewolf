﻿using System;
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
        [Display(Name = "Number of Players?")]
        [Range(7, 14,
        ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int PlayerCount { get; set; }
    }
}