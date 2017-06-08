using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Domain.Weapons
{
    public abstract class Weapon
    {
        [JsonIgnore]
        public BattleshipPlayer Owner { get; }

        [JsonProperty]
        [JsonConverter(typeof(StringEnumConverter))]
        public WeaponType WeaponType { get; set; }

        [JsonProperty]
        public Ship Ship { get; set; }

        [JsonProperty]
        public int EnergyRequired { get; }

        protected Weapon(BattleshipPlayer owner, int energyRequired)
        {
            this.EnergyRequired = energyRequired;
            this.Owner = owner;
        }

        public abstract void Shoot(List<Cell> targets, int currentRound);
    }

    public interface IWeaponTarget
    {
        bool LandShot();

        IWeaponTarget Neighbour(Direction direction);
    }

    public enum WeaponType
    {
        SingleShot,
        DoubleShot,
        ScatterShot,
        SeekerMissle,
        CrossShot
    }
}