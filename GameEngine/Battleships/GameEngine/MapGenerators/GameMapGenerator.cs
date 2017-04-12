using System;
using System.Collections.Generic;
using Domain.Games;
using Domain.Players;
using GameEngine.Common;
using GameEngine.Properties;

namespace GameEngine.MapGenerators
{
    public class GameMapGenerator
    {
        private readonly List<Player> _players;

        public GameMapGenerator(List<Player> players)
        {
            if (players.Count <= 1 || players.Count >= 3)
            {
                throw new ArgumentException("Number of players should be 2");
            }

            _players = players;
        }

        public GameMap GenerateGameMap(MapSizes mapSize)
        {
            var mapSizeNumber = GetMapSize(mapSize);
            var playerOneName = _players[0].Name;
            var playerTwoName = _players[1].Name;
            var gameMap = new GameMap(playerOneName, playerTwoName, mapSizeNumber, mapSizeNumber);
            GeneratePlayers(gameMap);
            return gameMap;
        }

        private int GetMapSize(MapSizes mapSize)
        {
            switch (mapSize)
            {
                case MapSizes.Small:
                    return Settings.Default.SmallMapSize;
                case MapSizes.Medium:
                    return Settings.Default.MediumMapSize;
                default:
                    return Settings.Default.LargeMapSize;
            }
        }

        private void GeneratePlayers(GameMap gameMap)
        {
            var playerOne = gameMap.GetBattleshipPlayer(PlayerType.One);
            _players[0].PlayerRegistered(playerOne);
            gameMap.RegisterPlayer(playerOne);

            var playerTwo = gameMap.GetBattleshipPlayer(PlayerType.Two);
            _players[1].PlayerRegistered(playerTwo);
            gameMap.RegisterPlayer(playerTwo);
        }
    }
}