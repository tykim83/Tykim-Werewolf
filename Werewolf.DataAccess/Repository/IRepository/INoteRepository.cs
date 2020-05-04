using System;
using System.Collections.Generic;
using System.Text;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository.IRepository
{
    public interface INoteRepository : IRepository<Note>
    {
        void Update(Note note);
    }
}