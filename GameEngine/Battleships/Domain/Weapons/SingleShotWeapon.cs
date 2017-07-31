using System.Collections.Generic;
using System.Linq;
using Domain.Maps;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;

namespace Domain.Weapons
{
    internal class SingleShotWeapon : Weapon
    {
        public SingleShotWeapon(BattleshipPlayer owner, int energyRequired, WeaponType weaponType) : base(owner,
            energyRequired, weaponType)
        {
        }

        public override void Shoot(List<Cell> targets, int currentRound)
        {
            var target = targets[0];

            Owner.ShotsFired++;

            var alreadyDestroyed = target.OccupiedBy?.Destroyed ?? false;
            if (target.LandShot())
            {
                Owner.ShotsHit++;

                Owner.AddPoints(Settings.Default.PointsHit);
                if (Owner.FirstShotLanded == int.MaxValue)
                {
                    Owner.FirstShotLanded = currentRound;
                }

                var destroyed = target.OccupiedBy?.Cells.All(x => x != null && x.Hit) ?? false;

                if (!alreadyDestroyed && destroyed)
                {
                    Owner.AddPoints(Settings.Default.PointsShipSunk);
                }
            }
        }

        public override string ToString()
        {
            return "Single Shot";
        }
    }
}