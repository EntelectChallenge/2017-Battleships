using System.Collections.Generic;
using System.Drawing;

using Domain.Games;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;

namespace GameEngine.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// Tells this command to perform the required action within the command transaction provided
        /// </summary>
        /// <param name="gameMap">The game map to make command calculations</param>
        /// <param name="player">The issuing player for this command</param>
        void PerformCommand(GameMap gameMap, BattleshipPlayer player); 
        
    }
}