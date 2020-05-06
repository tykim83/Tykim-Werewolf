using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.GameLogic.Interfaces;
using Werewolf.Models;
using Werewolf.Models.ViewModel;
using Werewolf.Utility;

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

            //Check if next turn is ready
            _playGame.CheckNextTurnReady(gameId);

            var notes = _unitOfWork.Note.GetAll(c => c.GameId == gameId && c.ApplicationUserId == claims.Value).ToList();

            PlayViewModel PlayVM = new PlayViewModel()
            {
                Character = _unitOfWork.GameUser.GetFirstOrDefault(filter: c => c.ApplicationUserId == claims.Value && c.GameId == gameId, includeProperties: "Game,ApplicationUser"),
                Opponents = _unitOfWork.GameUser.GetAll(filter: c => c.ApplicationUserId != claims.Value && c.GameId == gameId, includeProperties: "ApplicationUser", orderBy: q => q.OrderByDescending(c => c.IsAlive)),
                Notes = notes,
                VoteList = _unitOfWork.GameUser.GetAll(filter: c => c.GameId == gameId && c.IsAlive == true, includeProperties: "ApplicationUser").Select(c => c.ApplicationUser)
            };

            //Get the logs for the last turn
            PlayVM.Logs = _unitOfWork.Log.GetAll(c => c.GameId == gameId && c.Turn == PlayVM.Character.Game.TurnNumber - 1 && (c.Visible == SD.Everyone || c.Visible == PlayVM.Character.Role)).ToList();
            //PlayVM.Logs = logs.Where(c => c.Visible == SD.Everyone || c.Visible == PlayVM.Character.Role).ToList();

            //Get the already selected vote
            PlayVM.Vote = _unitOfWork.Vote.GetFirstOrDefault(c => c.ApplicationUserId == claims.Value && c.Turn == PlayVM.Character.Game.TurnNumber);
            //Get the list of already casted vote
            if (PlayVM.Character.Game.TurnType == SD.Night)
            {
                if (PlayVM.Character.Role == SD.Werewolf)
                {
                    //Get the list only for WEREWOLF for NIGHT
                    PlayVM.VoteCasted = _unitOfWork.Vote.GetAll(c => c.Role == SD.Werewolf && c.GameId == PlayVM.Character.GameId && c.Turn == PlayVM.Character.Game.TurnNumber);
                }
                else if (PlayVM.Character.Role == SD.Doctor)
                {
                    //Get the list only for DOCTOR for NIGHT
                    PlayVM.VoteCasted = _unitOfWork.Vote.GetAll(c => c.Role == SD.Doctor && c.GameId == PlayVM.Character.GameId && c.Turn == PlayVM.Character.Game.TurnNumber);
                }
                else if (PlayVM.Character.Role == SD.Seer)
                {
                    //Get the list only for SEER for NIGHT
                    PlayVM.VoteCasted = _unitOfWork.Vote.GetAll(c => c.Role == SD.Seer && c.GameId == PlayVM.Character.GameId && c.Turn == PlayVM.Character.Game.TurnNumber);
                }
            }
            else if (PlayVM.Character.Game.TurnType == SD.Day)
            {
                //get list for everyone during the Day
                PlayVM.VoteCasted = _unitOfWork.Vote.GetAll(c => c.GameId == PlayVM.Character.GameId && c.Turn == PlayVM.Character.Game.TurnNumber);
            }

            return View(PlayVM);
        }

        #region Note ApiCalls

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

        #endregion Note ApiCalls

        #region Vote ApiCalls

        [HttpPost]
        public IActionResult AddVote(int gameId, string role, string userVoteId)
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var turn = _unitOfWork.Game.Get(gameId).TurnNumber;

            var vote = new Vote()
            {
                GameId = gameId,
                ApplicationUserId = claims.Value,
                Role = role,
                Turn = turn,
                UserVotedId = userVoteId
            };

            _unitOfWork.Vote.Add(vote);
            _unitOfWork.Save();

            var voteFromDb = _unitOfWork.Vote.GetFirstOrDefault(c => c.GameId == gameId && c.ApplicationUserId == claims.Value && c.Turn == turn);
            return Json(new { success = true, message = "Saved successful.", id = voteFromDb.Id, nextTurn = _playGame.CheckNextTurnReady(voteFromDb.GameId) });
        }

        [HttpPost]
        public IActionResult UpdateVote(int id, string userVoteId)
        {
            var voteFromDb = _unitOfWork.Vote.GetFirstOrDefault(c => c.Id == id);

            if (voteFromDb == null)
            {
                return Json(new { success = false, message = "Something went wrong" });
            }

            voteFromDb.UserVotedId = userVoteId;

            _unitOfWork.Vote.Update(voteFromDb);

            return Json(new { success = true, message = "Saved successful.", nextTurn = _playGame.CheckNextTurnReady(voteFromDb.GameId) });
        }

        #endregion Vote ApiCalls

        #region NextTurn ApiCalls

        [HttpPost]
        public IActionResult CheckNextTurn(int gameId)
        {
            var ready = _playGame.CheckNextTurnReady(gameId);
            return Json(new { success = true, nextTurn = ready });
        }

        [HttpPost]
        public IActionResult NextTurn(int gameId)
        {
            _playGame.NextTurn(gameId);
            var redirectUrl = Url.Action(nameof(Index), new { gameId = gameId });
            return Json(new { success = true, url = redirectUrl });
        }

        #endregion NextTurn ApiCalls
    }
}