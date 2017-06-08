using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Properties;
using Domain.Ships;

namespace Domain.Weapons
{
    public class CrossShotWeapon : Weapon
    {
        public CrossShotWeapon(BattleshipPlayer owner, int energyRequired) : base(owner, energyRequired) { }

        public override void Shoot(List<Cell> targets, int currentRound)
        {
            Owner.ShotsHit++;
            foreach (var target in targets)
            {
                if (!target.LandShot()) continue;
                Owner.AddPoints(Settings.Default.PointsHit);
                if (Owner.FirstShotLanded == int.MaxValue)
                {
                    Owner.FirstShotLanded = currentRound;
                }
            }
        }

        public override string ToString()
        {
            return "Cross Shot";
        }
    }
}