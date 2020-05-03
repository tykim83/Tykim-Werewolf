using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.GameLogic.Interfaces;
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

            PlayViewModel PlayVM = new PlayViewModel()
            {
                Character = _unitOfWork.GameUser.GetFirstOrDefault(filter: c => c.ApplicationUserId == claims.Value && c.GameId == gameId, includeProperties: "Game,ApplicationUser"),
                Opponents = _unitOfWork.GameUser.GetAll(filter: c => c.ApplicationUserId != claims.Value && c.GameId == gameId, includeProperties: "ApplicationUser")
            };

            return View(PlayVM);
        }
    }
}