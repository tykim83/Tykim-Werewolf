using System;
using System.Collections.Generic;
using System.Text;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository.IRepository
{
    public interface IGameUserRepository : IRepository<GameUser>
    {
        int RegisteredPlayers(int gameId);

        IEnumerable<int> GameRegisteredPerUser(string userId);
    }
}