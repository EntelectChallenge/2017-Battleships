using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;

namespace Domain.Weapons
{
    public class SeekerMissleWeapon : Weapon
    {
        public SeekerMissleWeapon(BattleshipPlayer owner, int energyRequired, WeaponType weaponType) : base(owner, energyRequired, weaponType) { }

        public override void Shoot(List<Cell> targets, int currentRound)
        {
            
            //target.LandShot();
        }

        public override string ToString()
        {
            return "Seeker Missle";
        }
    }
}