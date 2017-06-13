using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;

namespace Domain.Weapons
{
    public class DoubleShotWeapon : Weapon
    {
        public DoubleShotWeapon(BattleshipPlayer owner, int energyRequired, WeaponType weaponType) : base(owner, energyRequired, weaponType) { }

        public override void Shoot(List<Cell> targets, int currentRound)
        {
            Owner.ShotsFired++;
            foreach (var target in targets)
            {
                var alreadyDestroyed = target.OccupiedBy?.Destroyed ?? false;
                if (!target.LandShot()) continue;

                Owner.ShotsHit++;
                Owner.AddPoints(Settings.Default.PointsHit);
                if (Owner.FirstShotLanded == int.MaxValue)
                {
                    Owner.FirstShotLanded = currentRound;
                }

                var destroyed = target.OccupiedBy?.Destroyed ?? false;

                if (!alreadyDestroyed && destroyed)
                {
                    Owner.AddPoints(Settings.Default.PointsShipSunk);
                }
            }
        }

        public override string ToString()
        {
            return "Double Shot";
        }
    }
}