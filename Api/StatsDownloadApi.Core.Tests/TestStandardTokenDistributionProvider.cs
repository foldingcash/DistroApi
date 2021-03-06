﻿namespace StatsDownloadApi.Core.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using StatsDownloadApi.Interfaces;
    using StatsDownloadApi.Interfaces.DataTransfer;

    [TestFixture]
    public class TestStandardTokenDistributionProvider
    {
        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new StandardTokenDistributionProvider();
        }

        private StandardTokenDistributionProvider systemUnderTest;

        [Test]
        public void CorrectDrift_WhenLargeDriftOver_FixesDrift()
        {
            var users = new List<DistroUser> { new("", 0, 0, 0.5M), new("", 0, 0, 0.50000009M) };

            systemUnderTest.CorrectDrift(1, users);

            Assert.That(users.Sum(user => user.Amount), Is.EqualTo(1));
        }

        [Test]
        public void CorrectDrift_WhenLargeDriftUnder_FixesDrift()
        {
            var users = new List<DistroUser> { new("", 0, 0, 0.5M), new("", 0, 0, 0.49999990M) };

            systemUnderTest.CorrectDrift(1, users);

            Assert.That(users.Sum(user => user.Amount), Is.EqualTo(1));
        }

        [Test]
        public void GetDistro_WhenDistributionHasNoReward_ThrowsException()
        {
            Assert.Throws<InvalidDistributionStateException>(() =>
                systemUnderTest.GetDistro(1, new[] { new FoldingUser("friendlyName1", "address1", 0, 0) }));
        }

        [Test]
        public void GetDistro_WhenInvoked_AmountAddedToEqualAmount()
        {
            IList<DistroUser> actual = systemUnderTest.GetDistro(1,
                new[]
                {
                    new FoldingUser("friendlyName1", "address1", 500, 0),
                    new FoldingUser("friendlyName2", "address2", 560, 0),
                    new FoldingUser("friendlyName3", "address3", 251, 0)
                });

            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual.Sum(user => user.Amount), Is.EqualTo(1));
        }

        [Test]
        public void GetDistro_WhenInvoked_AmountSubtractedToEqualAmount()
        {
            IList<DistroUser> actual = systemUnderTest.GetDistro(1,
                new[]
                {
                    new FoldingUser("friendlyName1", "address1", 508, 0),
                    new FoldingUser("friendlyName2", "address2", 557, 0),
                    new FoldingUser("friendlyName3", "address3", 931, 0)
                });

            Assert.That(actual.Count, Is.EqualTo(3));
            Assert.That(actual.Sum(user => user.Amount), Is.EqualTo(1));
        }

        [Test]
        public void GetDistro_WhenInvoked_CombinesDistroUsersByAddress()
        {
            IList<DistroUser> actual = systemUnderTest.GetDistro(1,
                new[]
                {
                    new FoldingUser("friendlyName1", "address1", 1, 2),
                    new FoldingUser("friendlyName2", "address2", 98, 96),
                    new FoldingUser("friendlyName1", "address1", 1, 2)
                });

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].BitcoinAddress, Is.EqualTo("address1"));
            Assert.That(actual[0].PointsGained, Is.EqualTo(2));
            Assert.That(actual[0].WorkUnitsGained, Is.EqualTo(4));
            Assert.That(actual[0].Amount, Is.EqualTo(0.02));
            Assert.That(actual[1].BitcoinAddress, Is.EqualTo("address2"));
            Assert.That(actual[1].PointsGained, Is.EqualTo(98));
            Assert.That(actual[1].WorkUnitsGained, Is.EqualTo(96));
            Assert.That(actual[1].Amount, Is.EqualTo(0.98));
        }

        [Test]
        public void GetDistro_WhenInvoked_ReturnsDistro()
        {
            IList<DistroUser> actual = systemUnderTest.GetDistro(1,
                new[]
                {
                    new FoldingUser("friendlyName1", "address1", 1, 2),
                    new FoldingUser("friendlyName2", "address2", 99, 98)
                });

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
                new[]
                {
                    new FoldingUser("friendlyName1", "address1", 500, 0),
                    new FoldingUser("friendlyName2", "address2", 560, 0)
                });

            Assert.That(actual.Count, Is.EqualTo(2));
            Assert.That(actual[0].Amount, Is.EqualTo(3655660.37735849));
            Assert.That(actual[1].Amount, Is.EqualTo(4094339.62264151));
        }
    }
}