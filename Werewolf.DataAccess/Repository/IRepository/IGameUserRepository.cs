using System;
using System.Collections.Generic;
using System.Text;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository.IRepository
{
    public interface IGameUserRepository : IRepository<GameUser>
    {
        void Update(GameUser gameUser);

        int RegisteredPlayers(int gameId);

        IEnumerable<int> GameRegisteredPerUser(string userId);

        IEnumerable<string> ListRegisteredUserPerGame(int gameId);
    }
}