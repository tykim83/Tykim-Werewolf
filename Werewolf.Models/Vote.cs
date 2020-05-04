using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Werewolf.Models
{
    public class Vote
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Game")]
        public int GameId { get; set; }

        public virtual Game Game { get; set; }

        [ForeignKey("ApplicationUser")]
        public string ApplicationUserId { get; set; }

        public virtual ApplicationUser ApplicationUser { get; set; }

        [ForeignKey("UserVoted")]
        public string UserVotedId { get; set; }

        public virtual ApplicationUser UserVoted { get; set; }
        public int Turn { get; set; }
        public string Role { get; set; }
    }
}