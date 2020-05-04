using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.GameLogic.Interfaces;
using Werewolf.Models;
using Werewolf.Models.ViewModel;

namespace Werewolf.Areas.Game.Controllers
{
    [Area("Game")]
    public class PlayController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlayGame _playGame;

        public PlayController(IUnitOfWork unitOfWork, IPlayGame playGame)
        {
            _unitOfWork = unitOfWork;
            _playGame = playGame;
        }

        public IActionResult Index(int gameId)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var notes = _unitOfWork.Note.GetAll(c => c.GameId == gameId && c.ApplicationUserId == claims.Value).ToList();

            PlayViewModel PlayVM = new PlayViewModel()
            {
                Character = _unitOfWork.GameUser.GetFirstOrDefault(filter: c => c.ApplicationUserId == claims.Value && c.GameId == gameId, includeProperties: "Game,ApplicationUser"),
                Opponents = _unitOfWork.GameUser.GetAll(filter: c => c.ApplicationUserId != claims.Value && c.GameId == gameId, includeProperties: "ApplicationUser"),
                Notes = notes
            };

            return View(PlayVM);
        }

        #region ApiCalls

        [HttpPost]
        public IActionResult AddNote(int gameId, string opponentId, string message)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var note = new Note()
            {
                ApplicationUserId = claims.Value,
                GameId = gameId,
                OpponentId = opponentId,
                Message = message
            };

            _unitOfWork.Note.Add(note);
            _unitOfWork.Save();

            var noteFromDb = _unitOfWork.Note.GetFirstOrDefault(c => c.GameId == gameId && c.ApplicationUserId == claims.Value && c.OpponentId == opponentId);
            return Json(new { success = true, message = "Saved successful.", id = noteFromDb.Id });
        }

        [HttpPost]
        public IActionResult UpdateNote(int id, string message)
        {
            var noteFromDb = _unitOfWork.Note.GetFirstOrDefault(c => c.Id == id);

            if (noteFromDb == null)
            {
                return Json(new { success = false, message = "Something went wrong" });
            }

            noteFromDb.Message = message;
            _unitOfWork.Note.Update(noteFromDb);

            return Json(new { success = true, message = "Saved successful." });
        }

        #endregion ApiCalls
    }
}