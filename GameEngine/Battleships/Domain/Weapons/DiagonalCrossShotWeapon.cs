using System;
using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;

namespace Domain.Weapons
{
    /// <summary>
    /// This will fire 5 shots in a 3 x 3 set of block given a center point.
    /// Each shot will be in the north-west, north-east, south-east and south-west corner 
    /// as well as the given center point of the block
    /// </summary>
    internal class DiagonalCrossShotWeapon : Weapon
    {
        public DiagonalCrossShotWeapon(BattleshipPlayer owner, int energyRequired, WeaponType weaponType) : base(owner, energyRequired, weaponType) { }

        public override void Shoot(List<Cell> targets, int currentRound)
        {

            if (targets.Count > 5)
            {
                throw new ArgumentException("Diagonal Cross Shot has to have 5 targets at most");
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
            return "Diagonal Cross Shot";
        }
    }
}