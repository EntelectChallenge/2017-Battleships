using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Domain.Exceptions;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;
using Newtonsoft.Json;
using InvalidOperationException = System.InvalidOperationException;

namespace Domain.Games
{
    public class GameMap
    {
        public const int GameLevel = 1;

        public const string GameVersion = "1.0.0";

        [JsonIgnore] private readonly Dictionary<PlayerType, BattleshipPlayer> _players;

        [JsonIgnore] private readonly Dictionary<PlayerType, PlayerMap> _opponentMaps;

        [JsonIgnore] private readonly Dictionary<PlayerType, PlayerMap> _playersMaps;

        [JsonProperty]
        public int Phase { get; set; }

        [JsonProperty]
        public IList<BattleshipPlayer> RegisteredPlayers { get; }

        [JsonProperty]
        public int CurrentRound { get; set; }

        [JsonProperty]
        public int MapSize { get; set; }

        [JsonProperty]
        public bool SuccessfulFirstRound { get; set; }

        [JsonProperty]
        public string ReasonForFirstRoundFailure { get; set; }

        private GameMap()
        {
            this._players = new Dictionary<PlayerType, BattleshipPlayer>();
            this._opponentMaps = new Dictionary<PlayerType, PlayerMap>();
            this._playersMaps = new Dictionary<PlayerType, PlayerMap>();
        }

        public GameMap(string playerOneName, string playerTwoName, int mapWidth, int mapHeight)
            : this()
        {
            this.MapSize = mapHeight;
            this.RegisteredPlayers = new List<BattleshipPlayer>();

            var playerOne = new BattleshipPlayer(playerOneName, 'A', PlayerType.One, mapHeight);
            var playerTwo = new BattleshipPlayer(playerTwoName, 'B', PlayerType.Two, mapHeight);

            this._players[PlayerType.One] = playerOne;
            this._players[PlayerType.Two] = playerTwo;

            var playerOneMap = new PlayerMap(mapWidth, mapHeight, playerOne);
            var playerTwoMap = new PlayerMap(mapWidth, mapHeight, playerTwo);

            this._opponentMaps[PlayerType.One] = playerTwoMap;
            this._opponentMaps[PlayerType.Two] = playerOneMap;

            this._playersMaps[PlayerType.One] = playerOneMap;
            this._playersMaps[PlayerType.Two] = playerTwoMap;

            this.Phase = 1;
        }

        public void Shoot(PlayerType player, List<Point> targets, WeaponType weaponType)
        {
            var actor = this._players[player];
            var targetMap = this._opponentMaps[player];
            var playerMap = this._playersMaps[player];

            if (!playerMap.IsReady())
            {
                throw new InvalidOperationException("All your ships must be placed before you are allowed to shoot");
            }

            var weapon = actor.GetWeapon(weaponType);

            if (actor.Energy >= weapon.EnergyRequired)
            {
                targetMap.Shoot(weapon, targets, CurrentRound);
                actor.Energy -= weapon.EnergyRequired;
            }
            else
            {
                throw new InsufficientEnergyException("The player does not have sufficient energy to use the selected weapon");
            }
        }


        public bool WasShipDestroyed(PlayerType player, Point point)
        {
            var targetMap = this._opponentMaps[player];
            var ship = targetMap.GetShipAtPoint(point);
            return ship != null && ship.Destroyed;
        }

        public void Place(PlayerType playerType, ShipType shipType, Point coordinate, Direction direction)
        {
            var player = this._players[playerType];

            var shipToPlace = player.Ships.Single(x => x.GetType() == shipType.EnumToType());

            if (shipToPlace.Placed)
            {
                throw new InvalidOperationException($"{shipType} has already been placed");
            }

            _playersMaps[playerType].Place(shipToPlace, coordinate, direction);
        }

        public bool CanPlace(PlayerType playerType, ShipType shipType, Point coordinate, Direction direction)
        {
            var player = this._players[playerType];

            var shipToPlace = player.Ships.Single(x => x.GetType() == shipType.EnumToType());

            return !shipToPlace.Placed && _playersMaps[playerType].CanPlace(shipToPlace, coordinate, direction);
        }

        public void CleanMapBeforePlace(PlayerType playerType)
        {
            var playerMap = _playersMaps[playerType];
            var player = _players[playerType];
            player.ResetShips();
            playerMap.ClearMap();
        }

        public BattleshipPlayer GetBattleshipPlayer(PlayerType playerType)
        {
            return this._players[playerType];
        }

        public BattleshipPlayer GetOppoentBattleshipPlayer(PlayerType playerType)
        {
            switch (playerType)
            {
                case PlayerType.One:
                    return this._players[PlayerType.Two];
                case PlayerType.Two:
                    return this._players[PlayerType.One];
                default:
                    return null;
            }
        }

        public PlayerMap GetOpponetMap(PlayerType playerType)
        {
            switch (playerType)
            {
                case PlayerType.One:
                    return this._playersMaps[PlayerType.Two];
                case PlayerType.Two:
                    return this._playersMaps[PlayerType.One];
                default:
                    return null;
            }
        }

        public PlayerMap GetPlayerMap(PlayerType playerType)
        {
            return _playersMaps[playerType];
        }

        public void RegisterPlayer(BattleshipPlayer player)
        {
            RegisteredPlayers.Add(player);
        }

        public void RemoveShield(PlayerType playerType)
        {
            var playerMap = _playersMaps[playerType];
            playerMap.RemoveShield();
        }

        public void PlaceShield(BattleshipPlayer player, Point centerPoint, int currentRound)
        {
            if (player.Shield.Active)
            {
                throw new Exception("A shiled is already active and cant be placed");
            }

            if (player.Shield.CurrentCharges == 0)
            {
                throw new Exception("The shield has no charge and cannot be applied");
            }

            var playerMap = _playersMaps[player.PlayerType];
            playerMap.PlaceShield(centerPoint, currentRound);
        }
    }
}