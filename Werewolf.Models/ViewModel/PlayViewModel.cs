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
    }
}