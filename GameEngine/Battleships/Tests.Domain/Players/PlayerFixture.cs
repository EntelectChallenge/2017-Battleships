using Domain.Games;
using Domain.Players;
using NUnit.Framework;

namespace Tests.Domain.Players
{
    [TestFixture]
    public class PlayerFixture
    {
        [Test]
        public void GivenName_WhenConstructing_SetsName()
        {
            var name = "TestName";

            var player = new BattleshipPlayer(name, 'A', PlayerType.One, 10);

            Assert.AreEqual(name, player.Name);
        }

        [Test]
        public void GivenNewPlayer_WhenConstructing_AddsAllShips()
        {
            var name = "TestName";

            var player = new BattleshipPlayer(name, 'A', PlayerType.One, 10);

            Assert.IsNotNull(player.Battleship);
            Assert.IsNotNull(player.Carrier);
            Assert.IsNotNull(player.Cruiser);
            Assert.IsNotNull(player.Destroyer);
            Assert.IsNotNull(player.Submarine);
        }
    }
}
