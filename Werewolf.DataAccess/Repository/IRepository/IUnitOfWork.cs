using System;
using System.Collections.Generic;
using System.Text;

namespace Werewolf.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        void Save();
    }
}