using System;
using System.Collections.Generic;
using System.Text;
using Werewolf.DataAccess.Repository.IRepository;

namespace Werewolf.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Game = new GameRepository(_db);
            GameUser = new GameUserRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            Note = new NoteRepository(_db);
            Vote = new VoteRepository(_db);
            Log = new LogRepository(_db);
        }

        public IGameRepository Game { get; private set; }
        public IGameUserRepository GameUser { get; private set; }

        public IApplicationUserRepository ApplicationUser { get; private set; }

        public INoteRepository Note { get; private set; }

        public IVoteRepository Vote { get; private set; }

        public ILogRepository Log { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}