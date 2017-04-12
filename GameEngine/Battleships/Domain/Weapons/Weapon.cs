using Domain.Maps;
using Domain.Players;
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

        protected Weapon(BattleshipPlayer owner)
        {
            this.Owner = owner;
        }

        public abstract bool Shoot(IWeaponTarget target);
    }

    public interface IWeaponTarget
    {
        bool LandShot();

        IWeaponTarget Neighbour(Direction direction);
    }

    public enum WeaponType
    {
        SingleShot
    }
}
