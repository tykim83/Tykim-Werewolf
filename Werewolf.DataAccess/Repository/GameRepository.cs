using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository
{
    public class GameRepository : Repository<Game>, IGameRepository
    {
        private readonly ApplicationDbContext _db;

        public GameRepository(ApplicationDbContext Db)
            : base(Db)
        {
            _db = Db;
        }

        public void Update(Game game)
        {
            var objFromDb = _db.Game.FirstOrDefault(c => c.Id == game.Id);

            objFromDb.Name = game.Name;
            objFromDb.Status = game.Status;
            objFromDb.Turn = game.Turn;
            objFromDb.TurnStarted = game.TurnStarted;
            objFromDb.PlayerCount = game.PlayerCount;

            _db.SaveChanges();
        }
    }
}