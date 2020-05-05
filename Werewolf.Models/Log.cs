using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Werewolf.Models
{
    public class Log
    {
        public int Id { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }

        public virtual Game Game { get; set; }
        public int Turn { get; set; }
        public string Message { get; set; }
    }
}