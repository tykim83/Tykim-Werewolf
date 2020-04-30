using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository
{
    public class GameUserRepository : Repository<GameUser>, IGameUserRepository
    {
        private readonly ApplicationDbContext _db;

        public GameUserRepository(ApplicationDbContext Db)
            : base(Db)
        {
            _db = Db;
        }

        public IEnumerable<int> GameRegisteredPerUser(string userId)
        {
            var items = _db.GameUser.Where(c => c.ApplicationUserId == userId).ToList();

            foreach (var item in items)
            {
                yield return item.GameId;
            }
        }

        public int RegisteredPlayers(int gameId)
        {
            return _db.GameUser.Where(c => c.GameId == gameId).Count();
        }
    }
}