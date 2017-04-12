using System;
using Domain.Maps;
using Domain.Weapons;

namespace Tests.Domain.Weapons.Stubs
{
    internal class WeaponTargetStub : IWeaponTarget
    {
        public bool LandShotCalled { get; private set; }

        public WeaponTargetStub()
        {
            this.LandShotCalled = false;
        }

        public bool LandShot()
        {
            this.LandShotCalled = true;
            return true;
        }

        public IWeaponTarget Neighbour(Direction direction)
        {
            throw new NotImplementedException();
        }
    }
}
