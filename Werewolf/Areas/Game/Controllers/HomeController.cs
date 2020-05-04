using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.GameLogic.Interfaces;
using Werewolf.Models;
using Werewolf.Models.ViewModel;
using Werewolf.Utility;

namespace Werewolf.Controllers
{
    [Area("Game")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPlayGame _playGame;

        public HomeController(IUnitOfWork unitOfWork, IPlayGame playGame)
        {
            _unitOfWork = unitOfWork;
            _playGame = playGame;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            //check if user is logged in
            if (claims != null)
            {
                //Get list of Started and Finished games
                var activeGames = _unitOfWork.GameUser.GetAll(filter: c => c.ApplicationUserId == claims.Value, includeProperties: "Game").Where(c => c.Game.Status == SD.Started || c.Game.Status == SD.Finished).ToList();

                return View(activeGames);
            }

            return View();
        }

        [Authorize]
        public IActionResult FindGame()
        {
            FindGameViewModel findGameVM = new FindGameViewModel()
            {
                Game = _unitOfWork.Game.GetAll(c => c.Status == SD.Pending),
                TotalRegisteredPlayersForGame = new Dictionary<int, int>(),
                AlreadyRegisteredGames = new List<int>()
            };

            foreach (var game in findGameVM.Game)
            {
                findGameVM.TotalRegisteredPlayersForGame.Add(game.Id, _unitOfWork.GameUser.RegisteredPlayers(game.Id));
            }

            //Get current user Id
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            findGameVM.AlreadyRegisteredGames = _unitOfWork.GameUser.GameRegisteredPerUser(claims.Value);

            return View(findGameVM);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult AddGame(Game game)
        {
            if (ModelState.IsValid)
            {
                game.TurnType = SD.Night;
                game.TurnNumber = 1;
                game.TurnStarted = DateTime.Now;
                _unitOfWork.Game.Add(game);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(FindGame));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        #region Api Calls

        [HttpPost]
        public IActionResult StartGame(int gameId)
        {
            //Game Init
            _playGame.GameInit(gameId);

            return Json(new { redirecturl = "/Game/Home/Index" });
        }

        [HttpPost]
        public IActionResult JoinGame(int gameId)
        {
            //Get current user Id
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Add user to game
            GameUser gameUsetToAdd = new GameUser()
            {
                ApplicationUserId = userId,
                GameId = gameId,
                IsAlive = true
            };

            _unitOfWork.GameUser.Add(gameUsetToAdd);
            _unitOfWork.Save();

            //Populate View Model
            FindGameTableRowViewModel findGameTableRowVM = new FindGameTableRowViewModel()
            {
                Game = _unitOfWork.Game.Get(gameId),
                TotalRegisteredPlayersForGame = new Dictionary<int, int>(),
                AlreadyRegisteredGames = _unitOfWork.GameUser.GameRegisteredPerUser(userId)
            };

            findGameTableRowVM.TotalRegisteredPlayersForGame.Add(gameId, _unitOfWork.GameUser.RegisteredPlayers(gameId));

            return PartialView("_FindGameTableRowPartial", findGameTableRowVM);
        }

        [HttpPost]
        public IActionResult QuitGame(int gameId)
        {
            //Get current user Id
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //Get GameUser from db
            GameUser gameUsetFromDb = _unitOfWork.GameUser.GetFirstOrDefault(c => c.ApplicationUserId == userId && c.GameId == gameId);

            _unitOfWork.GameUser.Remove(gameUsetFromDb);
            _unitOfWork.Save();

            //Populate View Model
            FindGameTableRowViewModel findGameTableRowVM = new FindGameTableRowViewModel()
            {
                Game = _unitOfWork.Game.Get(gameId),
                TotalRegisteredPlayersForGame = new Dictionary<int, int>(),
                AlreadyRegisteredGames = _unitOfWork.GameUser.GameRegisteredPerUser(userId)
            };

            findGameTableRowVM.TotalRegisteredPlayersForGame.Add(gameId, _unitOfWork.GameUser.RegisteredPlayers(gameId));

            return PartialView("_FindGameTableRowPartial", findGameTableRowVM);
        }

        #endregion Api Calls
    }
}