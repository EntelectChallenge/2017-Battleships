using System.Collections.Generic;
using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;

namespace Tests.Domain.Maps.Stubs
{
    internal class WeaponStub : Weapon
    {
        public bool ShootCalled { get; private set; }

        public List<Cell> Targets { get; private set; }

        public WeaponStub(BattleshipPlayer owner, int energyRequired)
            : base(owner, energyRequired)
        {
        }

        public override void Shoot(List<Cell> targets, int currentRound)
        {
            this.ShootCalled = true;

            this.Targets = targets;
        }
    }
}