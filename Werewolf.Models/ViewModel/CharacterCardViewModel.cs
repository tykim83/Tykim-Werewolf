using System;
using System.Collections.Generic;
using System.Text;

namespace Werewolf.Models.ViewModel
{
    public class CharacterCardViewModel
    {
        public GameUser Character { get; set; }
        public IEnumerable<Vote> VoteCasted { get; set; }
    }
}