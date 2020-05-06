using System;
using System.Collections.Generic;
using System.Text;

namespace Werewolf.Models.ViewModel
{
    public class PlayViewModel
    {
        public GameUser Character { get; set; }
        public IEnumerable<GameUser> Opponents { get; set; }
        public IEnumerable<Note> Notes { get; set; }

        //Vote section
        public Vote Vote { get; set; }

        public IEnumerable<ApplicationUser> VoteList { get; set; }
        public IEnumerable<Vote> VoteCasted { get; set; }
        public IEnumerable<Log> Logs { get; set; }
    }
}