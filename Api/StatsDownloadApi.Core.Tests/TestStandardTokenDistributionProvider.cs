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
            IList<DistroUser> actual = systemUnderTest.GetDistro(1,
                new[] { new FoldingUser("address1", 1, 2), new FoldingUser("address2", 99, 98) });

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("address1"));
            Assert.That(actual[0].PointsGained, Is.EqualTo(1));
            Assert.That(actual[0].WorkUnitsGained, Is.EqualTo(2));
            Assert.That(actual[0].Amount, Is.EqualTo(0.01));
            Assert.That(actual[1].BitcoinAddress, Is.EqualTo("address2"));
            Assert.That(actual[1].PointsGained, Is.EqualTo(99));
            Assert.That(actual[1].WorkUnitsGained, Is.EqualTo(98));
            Assert.That(actual[1].Amount, Is.EqualTo(0.99));
        }

        [Test]
        public void GetDistro_WhenProportionExceedsMaxPrecision_FloorsToHighestPrecision()
        {
            IList<DistroUser> actual = systemUnderTest.GetDistro(7750000,
                new[] { new FoldingUser("address1", 500, 0), new FoldingUser("address2", 560, 0) });

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].Amount, Is.EqualTo(3655660.37735849));
            Assert.That(actual[1].Amount, Is.EqualTo(4094339.62264151));
        }
    }
}