using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository
{
    public class VoteRepository : Repository<Vote>, IVoteRepository
    {
        private readonly ApplicationDbContext _db;

        public VoteRepository(ApplicationDbContext Db)
            : base(Db)
        {
            _db = Db;
        }

        public void Update(Vote vote)
        {
            var objFromDb = _db.Vote.FirstOrDefault(c => c.Id == vote.Id);

            objFromDb.UserVotedId = vote.UserVotedId;

            _db.SaveChanges();
        }
    }
}