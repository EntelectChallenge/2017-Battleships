using System;
using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;

namespace Domain.Weapons
{
    /// <summary>
    /// This will fire at points on the map give a center point and a direction.
    /// If the direction is North or South the shot will be vertical
    /// If the direction is East or West the shot will be horizontal
    /// </summary>
    internal class DoubleShotWeapon : Weapon
    {
        public DoubleShotWeapon(BattleshipPlayer owner, int energyRequired, WeaponType weaponType) : base(owner, energyRequired, weaponType) { }

        public override void Shoot(List<Cell> targets, int currentRound)
        {
            if (targets.Count > 2)
            {
                throw new ArgumentException("Double Shot has to have 2 targets at most");
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
            return "Double Shot";
        }
    }
}