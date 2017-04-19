using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Ships;
using Domain.Weapons;
using Newtonsoft.Json;

namespace Domain.Players
{
    public class BattleshipPlayer
    {
        [JsonProperty]
        public int FailedFirstPhaseCommands { get; set; }
        [JsonProperty]
        public string Name { get; }

        [JsonIgnore]
        public Submarine Submarine { get; }

        [JsonIgnore]
        public Destroyer Destroyer { get; }

        [JsonIgnore]
        public Cruiser Cruiser { get; }

        [JsonIgnore]
        public Carrier Carrier { get; }

        [JsonIgnore]
        public Battleship Battleship { get; }

        [JsonProperty]
        public List<Ship> Ships { get; }

        [JsonIgnore]
        public PlayerType PlayerType { get; private set; }

        [JsonProperty]
        public int Points { get; private set; }

        [JsonIgnore]
        public int Energy { get; set; }

        [JsonProperty]
        public bool Killed { get; set; }

        [JsonProperty]
        public bool IsWinner { get; set; }

        [JsonProperty]
        public int ShotsFired { get; set; }

        [JsonProperty]
        public int ShotsHit { get; set; }

        [JsonIgnore]
        public int FirstShotLanded { get; set; }

        public void AddPoints(int points)
        {
            this.Points += points;
        }

        public bool AllShippsPlaced()
        {
            return Ships.All(ship => ship.Placed);
        }

        [JsonProperty]
        public int ShipsRemaining
        {
            get { return this.Ships.Count(x => !x.Destroyed); }
        }

        public BattleshipPlayer(string name, char key, PlayerType type)
        {
            this.Name = name;
            this.PlayerType = type;
            this.Submarine = new Submarine(this, ShipType.Submarine);
            this.Destroyer = new Destroyer(this, ShipType.Destroyer);
            this.Cruiser = new Cruiser(this, ShipType.Cruiser);
            this.Carrier = new Carrier(this, ShipType.Carrier);
            this.Battleship = new Battleship(this, ShipType.Battleship);
            this.Points = 0;
            this.Energy = 0;
            this.IsWinner = false;
            this.FailedFirstPhaseCommands = 0;
            this.Ships = new List<Ship>
            {
                Submarine,
                Destroyer,
                Battleship,
                Carrier,
                Cruiser
            };
            this.Key = key;
            
            this.FirstShotLanded = int.MaxValue;
        }

        public override string ToString()
        {
            return Name;
        }

        public string PrintShips()
        { 
            return $"[{string.Join(",", Ships.Where(x => !x.Destroyed).Select(x => x.ToString()))}]";
        }

        public string PrintAllWeapons()
        {
            return string.Join(",", Ships.SelectMany(x => x.Weapons).Select(x => x.WeaponType.ToString()).Distinct());
        }

        public Weapon GetWeapon(WeaponType weaponType)
        {
            var weapon = Ships.Where(x => !x.Destroyed).SelectMany(x => x.Weapons).First(x => x.WeaponType == weaponType);
            if(weapon == null)
                throw new ArgumentException($"Player has no active ships capable of firing weapon {weaponType}");

            return weapon;
        }

        public char Key { get; set; }

        public void Killoff()
        {
            foreach (var ship in Ships)
            {
                foreach (var shipCell in ship.Cells)
                {
                    shipCell?.LandShot();
                }
            }
            Killed = true;
            Points = 0;
        }

        public void ResetShips()
        {
            foreach (var ship in Ships)
            {
                ship.Placed = false;
            }
        }
    }
}