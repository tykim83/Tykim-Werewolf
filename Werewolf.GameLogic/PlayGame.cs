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

        public bool CheckNextTurnReady(int gameId)
        {
            var gameFromDb = _unitOfWork.Game.GetFirstOrDefault(filter: c => c.Id == gameId, includeProperties: "Votes,Players");

            //Check Alive Players
            var alivePlayersCount = gameFromDb.Players.Where(c => c.IsAlive == true).Count();
            var aliveWerewolf = gameFromDb.Players.Where(c => c.IsAlive == true && c.Role == SD.Werewolf).Count();
            var aliveDoctor = gameFromDb.Players.Where(c => c.IsAlive == true && c.Role == SD.Doctor).Count();
            var aliveSeer = gameFromDb.Players.Where(c => c.IsAlive == true && c.Role == SD.Seer).Count();

            if (gameFromDb.TurnType == SD.Night)
            {
                //NIGHT TIME CHECK

                var totalVoteRequired = aliveWerewolf + aliveDoctor + aliveSeer;
                //GET ALL THE VOTES FOR THIS TURN EXCEPT NULL ONES
                var notNullVotes = gameFromDb.Votes.Where(c => c.Turn == gameFromDb.TurnNumber && c.UserVotedId != null).ToList();
                var werewolfsVote = notNullVotes.Where(c => c.Role == SD.Werewolf).ToList();

                //CHECK EVERYONE VOTED AND CHECK WEREWOLF VOTED SAME PERSON
                if (totalVoteRequired == notNullVotes.Count && werewolfsVote[0].UserVotedId == werewolfsVote[1].UserVotedId)
                {
                    //Works only with 2 werewolf
                    gameFromDb.IsNextTurnReady = true;
                    _unitOfWork.Save();
                    return true;
                }
            }
            else
            {
                //DAY TIME CHECK
            }

            gameFromDb.IsNextTurnReady = false;
            _unitOfWork.Save();
            return false;
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

        public void NextTurn(int gameId)
        {
            var gameFromDb = _unitOfWork.Game.GetFirstOrDefault(filter: c => c.Id == gameId, includeProperties: "Votes,Players");

            //Check for votes
            if (gameFromDb.TurnType == SD.Night)
            {
                //NIGHT TURN
            }
            else
            {
                //DAY TURN
            }

            //Change Turn
            gameFromDb.TurnNumber++;
            gameFromDb.TurnStarted = DateTime.Now;
            gameFromDb.TurnType = gameFromDb.TurnType == SD.Night ? SD.Day : SD.Night;
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