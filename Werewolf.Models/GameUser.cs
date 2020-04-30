using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Werewolf.Models
{
    public class GameUser
    {
        [Column(Order = 0)]
        [ForeignKey("Game")]
        public int GameId { get; set; }

        [Column(Order = 1)]
        [ForeignKey("User")]
        public string ApplicationUserId { get; set; }

        public virtual Game Game { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        public bool IsAlive { get; set; }
        public string Role { get; set; }
    }
}