using System;
using System.Collections.Generic;
using System.Text;

namespace Werewolf.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGameRepository Game { get; }
        IGameUserRepository GameUser { get; }
        IApplicationUserRepository ApplicationUser { get; }
        INoteRepository Note { get; }

        void Save();
    }
}