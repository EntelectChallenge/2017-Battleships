using Domain.Maps;
using Domain.Players;
using Domain.Ships;
using Domain.Weapons;

namespace Tests.Domain.Maps.Stubs
{
    internal class WeaponStub : Weapon
    {
        public bool ShootCalled { get; private set; }

        public IWeaponTarget Target { get; private set; }

        public WeaponStub(BattleshipPlayer owner)
            : base(owner)
        {
        }

        public override bool Shoot(IWeaponTarget target)
        {
            this.ShootCalled = true;

            this.Target = target;

            return true;
        }
    }
}