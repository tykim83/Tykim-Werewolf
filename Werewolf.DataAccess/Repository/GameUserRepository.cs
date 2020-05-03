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

        public IEnumerable<string> ListRegisteredUserPerGame(int gameId)
        {
            var items = _db.GameUser.Where(c => c.GameId == gameId).ToList();

            foreach (var item in items)
            {
                yield return item.ApplicationUserId;
            }
        }

        public int RegisteredPlayers(int gameId)
        {
            return _db.GameUser.Where(c => c.GameId == gameId).Count();
        }

        public void Update(GameUser gameUser)
        {
            var objFromDb = _db.GameUser.FirstOrDefault(c => c.ApplicationUserId == gameUser.ApplicationUserId && c.GameId == gameUser.GameId);

            objFromDb.IsAlive = gameUser.IsAlive;
            objFromDb.Role = gameUser.Role;

            _db.SaveChanges();
        }
    }
}