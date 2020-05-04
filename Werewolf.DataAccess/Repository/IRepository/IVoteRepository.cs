using System;
using System.Collections.Generic;
using System.Text;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository.IRepository
{
    public interface IVoteRepository : IRepository<Vote>
    {
        void Update(Vote vote);
    }
}