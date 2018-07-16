namespace StatsDownloadApi.Core.Tests
{
    using System.Collections.Generic;
    using Interfaces;
    using Interfaces.DataTransfer;
    using NUnit.Framework;

    [TestFixture]
    public class TestStandardTokenDistributionProvider
    {
        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new StandardTokenDistributionProvider();
        }

        private IStatsDownloadApiTokenDistributionService systemUnderTest;

        [Test]
        public void GetDistro_WhenInvoked_ReturnsDistro()
        {
            IList<DistroUser> actual = systemUnderTest.GetDistro(100,
                new[] { new FoldingUser("address1", 1, 1), new FoldingUser("address2", 99, 99) });

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("address1"));
            Assert.That(actual[0].PointsGained, Is.EqualTo(1));
            Assert.That(actual[0].WorkUnitsGained, Is.EqualTo(1));
            Assert.That(actual[1].BitcoinAddress, Is.EqualTo("address2"));
            Assert.That(actual[1].PointsGained, Is.EqualTo(99));
            Assert.That(actual[1].WorkUnitsGained, Is.EqualTo(99));
        }
    }
}