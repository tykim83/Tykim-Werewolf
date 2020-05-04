using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Werewolf.DataAccess.Repository;
using Werewolf.DataAccess.Repository.IRepository;
using Werewolf.GameLogic.Interfaces;
using Werewolf.Models;
using Werewolf.Utility;

namespace Werewolf.GameLogic
{
    public class PlayGame : IPlayGame
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlayGame(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void GameInit(int gameId)
        {
            //Assign Roles
            AssignRoles(gameId);

            var gameFromDb = _unitOfWork.Game.Get(gameId);

            //Set game to Started and turn to 1
            gameFromDb.Status = SD.Started;
            gameFromDb.TurnType = SD.Night;
            gameFromDb.TurnStarted = DateTime.Now;
            _unitOfWork.Save();
        }

        private void AssignRoles(int gameId)
        {
            //Get list of users Id for the game
            List<string> usersId = _unitOfWork.GameUser.ListRegisteredUserPerGame(gameId).ToList();

            var specialRoles = new List<string>() { SD.Werewolf, SD.Werewolf, SD.Seer, SD.Doctor };
            var random = new Random();
            int index;
            string userId;
            GameUser gameUserFromDb;

            while (usersId.Count > 0)
            {
                //Get a random Id
                index = random.Next(usersId.Count);
                userId = usersId[index];
                usersId.RemoveAt(index);

                //Get the GameUser from the database
                gameUserFromDb = _unitOfWork.GameUser.GetFirstOrDefault(c => c.ApplicationUserId == userId && c.GameId == gameId);

                //Assign Role
                if (specialRoles.Count > 0)
                {
                    gameUserFromDb.Role = specialRoles[0];
                    specialRoles.RemoveAt(0);
                }
                else
                {
                    gameUserFromDb.Role = SD.Villager;
                }

                //Save Changes
                _unitOfWork.GameUser.Update(gameUserFromDb);
            }
        }
    }
}