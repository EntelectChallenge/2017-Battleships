using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;

namespace Domain.Weapons
{
    /// <summary>
    /// This will fire a shot at the calculated position, given a center point in a 5 x 5 block of cells
    /// The target is the closest ship cell in a 5 x 5 block of cells, regardless whether the cell has already been hit
    /// </summary>
    internal class SeekerMissileWeapon : Weapon
    {
        public SeekerMissileWeapon(BattleshipPlayer owner, int energyRequired, WeaponType weaponType) : base(owner, energyRequired, weaponType) { }

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

                var destroyed = target.OccupiedBy?.Destroyed ?? false;

                if (!alreadyDestroyed && destroyed)
                {
                    Owner.AddPoints(Settings.Default.PointsShipSunk);
                }
            }
        }

        public override string ToString()
        {
            return "Seeker Missile";
        }
    }
}