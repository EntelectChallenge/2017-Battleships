using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Games;
using Domain.Maps;
using Domain.Properties;
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

        [JsonProperty]
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

        [JsonIgnore] private int _mapSize;

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

        public BattleshipPlayer(string name, char key, PlayerType type, int mapSize)
        {
            this._mapSize = mapSize;
            this.Name = name;
            this.PlayerType = type;
            this.Submarine = new Submarine(this, ShipType.Submarine,
                new SeekerMissleWeapon(this, EnergyRequiredForWeapon(WeaponType.SeekerMissle),
                    WeaponType.SeekerMissle));
            this.Destroyer = new Destroyer(this, ShipType.Destroyer,
                new DoubleShotWeapon(this, EnergyRequiredForWeapon(WeaponType.DoubleShot), WeaponType.DoubleShot));
            this.Cruiser = new Cruiser(this, ShipType.Cruiser,
                new CrossShotWeapon(this, EnergyRequiredForWeapon(WeaponType.CrossShot), WeaponType.CrossShot));
            this.Carrier = new Carrier(this, ShipType.Carrier,
                new CornerShotWeapon(this, EnergyRequiredForWeapon(WeaponType.CornerShot), WeaponType.CornerShot));
            this.Battleship = new Battleship(this, ShipType.Battleship,
                new DiagonalCrossShotWeapon(this, EnergyRequiredForWeapon(WeaponType.DiagonalCrossShot),
                    WeaponType.DiagonalCrossShot));
            this.Points = 0;
            this.Energy = Settings.Default.SmallMapSize == mapSize ? 2 : Settings.Default.MediumMapSize == mapSize ? 3 : 4;
            this.IsWinner = false;
            this.FailedFirstPhaseCommands = 0;
            this.Ships = new List<Ship>
            {
                this.Submarine,
                this.Destroyer,
                this.Battleship,
                this.Carrier,
                this.Cruiser
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

        public string PrintAvailableWeapons()
        {
            return
                $"[{string.Join(",", Ships.Where(x => !x.Destroyed).SelectMany(x => x.Weapons).Select(x => x.WeaponType.ToString()).Distinct())}]";
        }

        public Weapon GetWeapon(WeaponType weaponType)
        {
            var weapon = Ships.Where(x => !x.Destroyed).SelectMany(x => x.Weapons)
                .FirstOrDefault(x => x.WeaponType == weaponType);
            if (weapon == null)
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

        //2 energy for small map
        //3 energy for medium map
        //4 energy for large map
        private int EnergyRequiredForWeapon(WeaponType type)
        {
            var energyRequired = 1;
            int rounds;
            switch (type)
            {
                //8 rounds
                case WeaponType.DoubleShot:
                    rounds = 8;
                    energyRequired = _mapSize == Settings.Default.SmallMapSize
                        ? rounds * Settings.Default.EnergySmallMap
                        : (_mapSize == Settings.Default.MediumMapSize
                            ? rounds * Settings.Default.EnergyMediumMap
                            : rounds * Settings.Default.EnergyLargMap);
                    break;
                //10 Rounds
                case WeaponType.CornerShot:
                    rounds = 10;
                    energyRequired = _mapSize == Settings.Default.SmallMapSize
                        ? rounds * Settings.Default.EnergySmallMap
                        : (_mapSize == Settings.Default.MediumMapSize
                            ? rounds * Settings.Default.EnergyMediumMap
                            : rounds * Settings.Default.EnergyLargMap);
                    break;
                //10 Rounds
                case WeaponType.SeekerMissle:
                    rounds = 10;
                    energyRequired = _mapSize == Settings.Default.SmallMapSize
                        ? rounds * Settings.Default.EnergySmallMap
                        : (_mapSize == Settings.Default.MediumMapSize
                            ? rounds * Settings.Default.EnergyMediumMap
                            : rounds * Settings.Default.EnergyLargMap);
                    break;
                //12 Rounds
                case WeaponType.DiagonalCrossShot:
                    rounds = 12;
                    energyRequired = _mapSize == Settings.Default.SmallMapSize
                        ? rounds * Settings.Default.EnergySmallMap
                        : (_mapSize == Settings.Default.MediumMapSize
                            ? rounds * Settings.Default.EnergyMediumMap
                            : rounds * Settings.Default.EnergyLargMap);
                    break;
                //14 Rounds
                case WeaponType.CrossShot:
                    rounds = 14;
                    energyRequired = _mapSize == Settings.Default.SmallMapSize
                        ? rounds * Settings.Default.EnergySmallMap
                        : (_mapSize == Settings.Default.MediumMapSize
                            ? rounds * Settings.Default.EnergyMediumMap
                            : rounds * Settings.Default.EnergyLargMap);
                    break;
            }
            return energyRequired;
        }
    }
}