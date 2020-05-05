using System;
using System.Collections.Generic;
using System.Text;

namespace Werewolf.GameLogic.Interfaces
{
    public interface IPlayGame
    {
        void GameInit(int gameId);

        bool CheckNextTurnReady(int gameId);
    }
}