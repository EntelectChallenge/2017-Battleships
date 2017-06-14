using System;
using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;

namespace Domain.Weapons
{
    /// <summary>
    /// This will fire 4 shots in a 3 x 3 set of block given a center point.
    /// Each shot will be in the north-west, north-east, south-east and south-west corner of the block
    /// </summary>
    internal class CornerShotWeapon : Weapon
    {
        public CornerShotWeapon(BattleshipPlayer owner, int energyRequired, WeaponType weaponType) : base(owner, energyRequired, weaponType) { }

        public override void Shoot(List<Cell> targets, int currentRound)
        {
            if (targets.Count > 4)
            {
                throw new ArgumentException("Corner shot has to have 4 targets at most");
            }
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
            return "Scatter Shot";
        }
    }
}