﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.Models;
using Werewolf.Utility;

namespace Werewolf.Controllers
{
    [Area("Game")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult FindGame()
        {
            var games = _unitOfWork.Game.GetAll(c => c.Status == SD.Pending);

            return View(games);
        }

        [Authorize]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult AddGame(Game game)
        {
            if (ModelState.IsValid)
            {
                game.Turn = 0;
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
    }
}