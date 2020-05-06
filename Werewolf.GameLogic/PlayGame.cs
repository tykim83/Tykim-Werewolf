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

            if (gameFromDb.TurnType == SD.Night)
            {
                //NIGHT TIME CHECK

                //Check Alive Players

                var aliveWerewolf = gameFromDb.Players.Where(c => c.IsAlive == true && c.Role == SD.Werewolf).Count();
                var aliveDoctor = gameFromDb.Players.Where(c => c.IsAlive == true && c.Role == SD.Doctor).Count();
                var aliveSeer = gameFromDb.Players.Where(c => c.IsAlive == true && c.Role == SD.Seer).Count();

                var totalVoteRequired = aliveWerewolf + aliveDoctor + aliveSeer;
                //GET ALL THE VOTES FOR THIS TURN EXCEPT NULL ONES
                var notNullVotes = gameFromDb.Votes.Where(c => c.Turn == gameFromDb.TurnNumber && c.UserVotedId != null).ToList();
                var werewolfsVote = notNullVotes.Where(c => c.Role == SD.Werewolf).ToList();

                //CHECK EVERYONE VOTED AND CHECK WEREWOLF VOTED SAME PERSON
                if ((totalVoteRequired == notNullVotes.Count && werewolfsVote[0].UserVotedId == werewolfsVote[1].UserVotedId) || aliveWerewolf == 1)
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
                var alivePlayersCount = gameFromDb.Players.Where(c => c.IsAlive == true).Count();

                //GET ALL THE VOTES FOR THIS TURN EXCEPT NULL ONES
                var notNullVotes = gameFromDb.Votes.Where(c => c.Turn == gameFromDb.TurnNumber && c.UserVotedId != null).ToList().Count();

                if (alivePlayersCount <= notNullVotes)
                {
                    gameFromDb.IsNextTurnReady = true;
                    _unitOfWork.Save();
                    return true;
                }
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
            var gameFromDb = _unitOfWork.Game.GetFirstOrDefault(filter: c => c.Id == gameId, includeProperties: "Players");
            var votesFromDb = _unitOfWork.Vote.GetAll(c => c.GameId == gameId && c.Turn == gameFromDb.TurnNumber, includeProperties: "UserVoted");

            if (gameFromDb.TurnType == SD.Night)
            {
                //NIGHT TURN

                //Check for votes
                var votes = votesFromDb
                    .GroupBy(c => c.Role)
                    .Select(c => new
                    {
                        role = c.Key,
                        votedId = c.Where(d => d.Role == c.Key).First().UserVoted.Id,
                        votedName = c.Where(d => d.Role == c.Key).First().UserVoted.Name
                    })
                    .ToList();

                //var id = votes.FirstOrDefault(c => c.role == SD.Doctor).votedId;
                //var role = votes.FirstOrDefault(c => c.role == SD.Doctor).role;
                //var name = votes.FirstOrDefault(c => c.role == SD.Doctor).votedName;

                if (votes.FirstOrDefault(c => c.role == SD.Doctor).votedId == votes.FirstOrDefault(c => c.role == SD.Werewolf).votedId)
                {
                    //Doctor Saved player
                    var log = new Log()
                    {
                        GameId = gameId,
                        Turn = gameFromDb.TurnNumber,
                        Message = votes.FirstOrDefault(c => c.role == SD.Doctor).votedName + " was saved by the Doctor",
                        Visible = SD.Everyone
                    };

                    _unitOfWork.Log.Add(log);
                }
                else
                {
                    //Werewolf killed player
                    var log = new Log()
                    {
                        GameId = gameId,
                        Turn = gameFromDb.TurnNumber,
                        Message = votes.FirstOrDefault(c => c.role == SD.Werewolf).votedName + " was Killed",
                        Visible = SD.Everyone
                    };

                    _unitOfWork.Log.Add(log);

                    KillPlayer(gameId, votes.FirstOrDefault(c => c.role == SD.Werewolf).votedId);
                }

                if (votes.FirstOrDefault(c => c.role == SD.Seer) != null)
                {
                    var id = votes.FirstOrDefault(c => c.role == SD.Seer).votedId;
                    var role = gameFromDb.Players.Where(c => c.ApplicationUserId == id).FirstOrDefault().Role;
                    var votedName = votes.FirstOrDefault(c => c.role == SD.Doctor).votedName;
                    //Add log for Seer
                    var seerLog = new Log()
                    {
                        GameId = gameId,
                        Turn = gameFromDb.TurnNumber,
                        Message = votedName + " is a " + role,
                        Visible = SD.Seer
                    };

                    _unitOfWork.Log.Add(seerLog);
                }
            }
            else
            {
                //DAY TURN

                //Check for votes
                var votes = votesFromDb
                    .GroupBy(c => c.UserVotedId)
                    .Select(c => new
                    {
                        votedId = c.Key,
                        votedName = c.Where(d => d.UserVotedId == c.Key).First().UserVoted.Name,
                        count = c.Where(d => d.UserVotedId == c.Key).Count()
                    })
                    .OrderByDescending(c => c.count)
                    .ToList();

                //Check who got the most votes
                int playerIndex;
                if (votes.Count > 1)
                {
                    if (votes[0].count > votes[1].count)
                    {
                        playerIndex = 0;
                    }
                    else
                    {
                        Random rnd = new Random();
                        playerIndex = rnd.Next(0, 1);
                    }
                }
                else
                {
                    playerIndex = 0;
                }

                //Kill player
                KillPlayer(gameId, votes[playerIndex].votedId);

                //log
                var log = new Log()
                {
                    GameId = gameId,
                    Turn = gameFromDb.TurnNumber,
                    Message = votes[playerIndex].votedName + " was Killed",
                    Visible = SD.Everyone
                };

                _unitOfWork.Log.Add(log);
            }

            //Change Turn
            gameFromDb.TurnNumber++;
            gameFromDb.TurnStarted = DateTime.Now;
            gameFromDb.TurnType = gameFromDb.TurnType == SD.Night ? SD.Day : SD.Night;
            gameFromDb.IsNextTurnReady = false;
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

        private void KillPlayer(int gameId, string playerId)
        {
            var gameUserFromDb = _unitOfWork.GameUser.GetFirstOrDefault(c => c.GameId == gameId && c.ApplicationUserId == playerId);

            gameUserFromDb.IsAlive = false;
            _unitOfWork.GameUser.Update(gameUserFromDb);
        }
    }
}