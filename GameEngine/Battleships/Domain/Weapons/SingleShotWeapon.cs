using Domain.Players;

namespace Domain.Weapons
{
    internal class SingleShotWeapon : Weapon
    {
        public SingleShotWeapon(BattleshipPlayer owner)
            : base(owner) {}

        public override bool Shoot(IWeaponTarget target)
        {
            return target.LandShot();
        }

        public override string ToString()
        {
            return "Single Shot";
        }
    }
}