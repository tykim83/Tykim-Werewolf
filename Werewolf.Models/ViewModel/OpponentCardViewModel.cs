using System;
using System.Collections.Generic;
using System.Text;

namespace Werewolf.Models.ViewModel
{
    public class OpponentCardViewModel
    {
        public GameUser Opponent { get; set; }
        public string CharacterRole { get; set; }
        public Note Note { get; set; }
    }
}