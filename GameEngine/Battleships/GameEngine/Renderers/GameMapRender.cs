using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Domain.Games;
using Domain.JSON;
using Domain.Maps;
using GameEngine.Properties;
using Newtonsoft.Json;

namespace GameEngine.Renderers
{
    public class GameMapRender
    {
        protected readonly GameMap GameMap;
        private readonly Boolean minify;

        public GameMapRender(GameMap gameMap)
            : this(gameMap, false)
        {
        }

        public GameMapRender(GameMap gameMap, bool minify)
        {
            GameMap = gameMap;
            this.minify = minify;
        }

        public StringBuilder RenderJsonGameState(PlayerType playerType)
        {
            return
                new StringBuilder(JsonConvert.SerializeObject(CreatePlayerGameState(playerType), minify ? Formatting.None : Formatting.Indented));
        }

        public PlayerGameState CreatePlayerGameState(PlayerType playerType)
        {
            if (playerType == PlayerType.Both)
            {
                return new PlayerGameState
                {
                    Player1Map = GameMap.GetPlayerMap(PlayerType.One),
                    Player2Map = GameMap.GetPlayerMap(PlayerType.Two),
                    GameLevel = GameMap.GameLevel,
                    GameVersion = GameMap.GameVersion,
                    MapDimension = GameMap.MapSize,
                    Round = GameMap.CurrentRound,
                    Phase = GameMap.Phase
                };
            }
            var opponentsMap = GameMap.GetOpponetMap(playerType);
            var playerMap = GameMap.GetPlayerMap(playerType);
            var playerGameState = new PlayerGameState
            {
                PlayerMap = playerMap,
                OpponentMap = new OpponentPlayerMap
                {
                    Ships = opponentsMap.Owner.Ships.Select(x => new OpponentShip()
                    {
                        ShipType = x.ShipType,
                        Destroyed = x.Destroyed
                    }).ToList(),
                    Cells = opponentsMap.Cells.Select(x => new OpponentCell
                    {
                        Damaged = x.Damaged,
                        Missed = x.Missed,
                        Y = x.Y,
                        X = x.X,
                        ShieldHit = x.ShieldHit
                    }).ToList(),
                    Name = opponentsMap.Owner.Name,
                    Alive = !opponentsMap.Owner.Killed,
                    Points = opponentsMap.Owner.Points
                },
                GameLevel = GameMap.GameLevel,
                GameVersion = GameMap.GameVersion,
                MapDimension = GameMap.MapSize,
                Round = GameMap.CurrentRound,
                Phase = GameMap.Phase
            };

            return playerGameState;
        }

        public virtual StringBuilder RenderTextGameState(PlayerType playerType, bool failure = false,
            bool renderLegend = false)
        {
            var sb = new StringBuilder();
            sb.Append(RenderMap(playerType, failure, renderLegend));
            sb.Append(RenderPlayerInfo(playerType));

            return sb;
        }

        public StringBuilder RenderMap(PlayerType playerType, bool failure, bool renderLegend)
        {
            var sb = new StringBuilder();

            sb.AppendLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            sb.AppendLine($"Game Version : {Settings.Default.GameEngineVersion}");
            sb.AppendLine($"Game Level : {Settings.Default.GameEngineLevel}");
            sb.AppendLine($"Round Number : {GameMap.CurrentRound}");
            sb.AppendLine($"Map Dimensions : {GameMap.MapSize} x {GameMap.MapSize}");
            sb.AppendLine($"Phase : {GameMap.Phase}");
            sb.AppendLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
            sb.AppendLine();
            sb.Append(Repeat('#', GameMap.MapSize));
            sb.AppendLine(" Player");
            var mapWidth = GameMap.MapSize - 1;
            var playerMap = GameMap.GetPlayerMap(playerType);
            for (var y = mapWidth; y >= 0; y--)
            {
                for (var x = 0; x <= mapWidth; x++)
                {
                    var cell = playerMap.GetCellAtPoint(new Point(x, y));
                    sb.Append(cell.Shielded && cell.ShieldHit ? '@' 
                        : cell.Occupied ? GetShipSymbol(cell.OccupiedBy.ShipType, cell.Damaged) : '~');
                }
                sb.AppendLine();
            }
            sb.Append(Repeat('#', GameMap.MapSize));
            sb.AppendLine(" Opponent");
            var opponentMap = GameMap.GetOpponetMap(playerType);
            for (var y = mapWidth; y >= 0; y--)
            {
                for (var x = 0; x <= mapWidth; x++)
                {
                    var cell = opponentMap.GetCellAtPoint(new Point(x, y));
                    sb.Append(cell.Shielded && cell.ShieldHit ? '@' 
                        : cell.Damaged ? '*' 
                        : cell.Missed ? '!' : '~');
                }
                sb.AppendLine();
            }
            sb.AppendLine(Repeat('#', GameMap.MapSize));
            sb.AppendLine();

            if (renderLegend)
            {
                sb.AppendLine();
                sb.AppendLine(Repeat('-', GameMap.MapSize));
                sb.AppendLine();

                sb.AppendLine("Legend: ");
                sb.AppendLine("~: Water");
                sb.AppendLine("!: Miss");
                sb.AppendLine("*: Hit");
                sb.AppendLine("@: Shield hit");
                sb.AppendLine("[B,C,R,S,D]: Healthy Ships");
                sb.AppendLine("[b,c,r,s,d]: Damaged Ships");
                sb.AppendLine("B-b: BattleShip");
                sb.AppendLine("C-c: Carrier");
                sb.AppendLine("R-r: Cruiser");
                sb.AppendLine("S-s: Submarine");
                sb.AppendLine("D-d: Destroyer");

                sb.AppendLine();
                sb.AppendLine(Repeat('-', GameMap.MapSize));
                sb.AppendLine();
            }

            return sb;
        }

        public StringBuilder RenderPlayerInfo(PlayerType playerType)
        {
            var sb = new StringBuilder();

            var player = GameMap.GetBattleshipPlayer(playerType);
            var opponent = GameMap.GetOppoentBattleshipPlayer(playerType);

            if (player == null)
            {
                throw new ArgumentException("The player you were looking for is not registered");
            }

            //Prints player info only
            sb.AppendLine("---------------------------")
                .AppendLine($"Player Name: {player.Name}")
                .AppendLine($"Available Energy: {player.Energy}")
                .AppendLine($"Shots: {player.ShotsFired}")
                .AppendLine($"Hit: {player.ShotsHit}")
                .AppendLine($"Points: {player.Points}")
                .AppendLine($"Arsenal: {player.PrintAvailableWeapons()}")
                .AppendLine($"Ships: {player.PrintShips()}")
                .AppendLine($"Status: {(!player.Killed ? "Alive" : "Dead")}")
                .AppendLine($"Shield: (Status: {(player.Shield.Active ? "Activated" : "Deactivated")}, " +
                            $"Charges: {player.Shield.CurrentCharges}), " +
                            $"Center Point: {(player.Shield.Active ? "X:" + player.Shield.CenterPoint.X + "," + "Y:" + player.Shield.CenterPoint.Y : "No Shield")}");

            //Prints opponents info
            sb.AppendLine("---------------------------")
                .AppendLine($"Opponent: {opponent.Name}")
                .AppendLine($"Points: {opponent.Points}")
                .AppendLine($"Ships: {opponent.PrintShips()}")
                .AppendLine($"Status: {(!opponent.Killed ? "Alive" : "Dead")}")
                .AppendLine("---------------------------");


            return sb;
        }

        private string Repeat(char symbol, int length)
        {
            var repeatedSymbol = "";
            for (var i = 0; i < length; i++)
            {
                repeatedSymbol += symbol.ToString();
            }
            return repeatedSymbol;
        }

        public static char GetShipSymbol(ShipType shipType, bool damaged)
        {
            switch (shipType)
            {
                case ShipType.Battleship:
                    return damaged ? 'b' : 'B';
                case ShipType.Carrier:
                    return damaged ? 'c' : 'C';
                case ShipType.Cruiser:
                    return damaged ? 'r' : 'R';
                case ShipType.Destroyer:
                    return damaged ? 'd' : 'D';
                case ShipType.Submarine:
                    return damaged ? 's' : 'S';
                default:
                    throw new ArgumentException("No such ship type");
            }
        }
    }
}