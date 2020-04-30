using System;
using System.Collections.Generic;
using System.Text;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository.IRepository
{
    public interface IGameRepository : IRepository<Game>
    {
        void Update(Game game);
    }
}