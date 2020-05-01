using System;
using System.Collections.Generic;
using System.Text;

namespace Werewolf.Models.ViewModel
{
    public class FindGameTableRowViewModel
    {
        public Game Game { get; set; }
        public IDictionary<int, int> TotalRegisteredPlayersForGame { get; set; }
        public IEnumerable<int> AlreadyRegisteredGames { get; set; }
    }
}