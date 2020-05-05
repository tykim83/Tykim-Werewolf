using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository
{
    public class LogRepository : Repository<Log>, ILogRepository
    {
        private readonly ApplicationDbContext _db;

        public LogRepository(ApplicationDbContext Db)
            : base(Db)
        {
            _db = Db;
        }
    }
}