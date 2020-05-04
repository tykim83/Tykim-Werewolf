using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.Models;

namespace Werewolf.DataAccess.Repository
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        private readonly ApplicationDbContext _db;

        public NoteRepository(ApplicationDbContext Db)
            : base(Db)
        {
            _db = Db;
        }

        public void Update(Note note)
        {
            var objFromDb = _db.Note.FirstOrDefault(c => c.Id == note.Id);

            objFromDb.Message = note.Message;

            _db.SaveChanges();
        }
    }
}