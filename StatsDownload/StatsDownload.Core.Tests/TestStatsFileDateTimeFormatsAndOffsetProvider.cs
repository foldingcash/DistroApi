namespace StatsDownload.Core.Tests
{
    using System;
    using Implementations;
    using Interfaces;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TestStatsFileDateTimeFormatsAndOffsetProvider
    {
        [SetUp]
        public void SetUp()
        {
            statsFileDateTimeFormatsAndOffsetSettingsMock =
                Substitute.For<IStatsFileDateTimeFormatsAndOffsetSettings>();
            statsFileDateTimeFormatsAndOffsetSettingsMock
                .GetStatsFileTimeZoneAndOffsetSettings().Returns("ZONE1=-1;ZONE2=0;ZONE3=1;");

            systemUnderTest =
                NewStatsFileDateTimeFormatsAndOffsetProvider(statsFileDateTimeFormatsAndOffsetSettingsMock);
        }

        private IStatsFileDateTimeFormatsAndOffsetSettings statsFileDateTimeFormatsAndOffsetSettingsMock;

        private IStatsFileDateTimeFormatsAndOffsetService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewStatsFileDateTimeFormatsAndOffsetProvider(null));
        }

        [Test]
        public void GetStatsFileDateTimeZoneAndOffset_WhenInvoked_ReturnsDateTimeFormatsAndOffsetsConstants()
        {
            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.IsSupersetOf(actual, Constants.StatsFile.DateTimeFormatsAndOffset);
        }

        [TestCase("malformed")]
        public void GetStatsFileDateTimeZoneAndOffset_WhenMalformedSettings_ReturnsDateTimeFormats(string settings)
        {
            statsFileDateTimeFormatsAndOffsetSettingsMock
                .GetStatsFileTimeZoneAndOffsetSettings().Returns(settings);

            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.AreEquivalent(actual, Constants.StatsFile.DateTimeFormatsAndOffset);
        }

        [TestCase(null)]
        [TestCase("")]
        public void GetStatsFileDateTimeZoneAndOffset_WhenSettingsEmpty_ReturnsDateTimeFormats(string settings)
        {
            statsFileDateTimeFormatsAndOffsetSettingsMock
                .GetStatsFileTimeZoneAndOffsetSettings().Returns(settings);

            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.AreEquivalent(actual, Constants.StatsFile.DateTimeFormatsAndOffset);
        }

        [Test]
        public void GetStatsFileDateTimeZoneAndOffsetSettings_WhenInvoked_ReturnsConfiguredDateTimeFormatsAndOffsets()
        {
            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.IsSupersetOf(actual, new[]
            {
                ("ddd MMM  d HH:mm:ss ZONE1 yyyy", -1),
                ("ddd MMM dd HH:mm:ss ZONE1 yyyy", -1),
                ("ddd MMM  d HH:mm:ss ZONE2 yyyy", 0),
                ("ddd MMM dd HH:mm:ss ZONE2 yyyy", 0),
                ("ddd MMM  d HH:mm:ss ZONE3 yyyy", 1),
                ("ddd MMM dd HH:mm:ss ZONE3 yyyy", 1)
            });
        }

        private IStatsFileDateTimeFormatsAndOffsetService NewStatsFileDateTimeFormatsAndOffsetProvider(
            IStatsFileDateTimeFormatsAndOffsetSettings statsFileDateTimeFormatsAndOffsetSettings)
        {
            return new StatsFileDateTimeFormatsAndOffsetProvider(statsFileDateTimeFormatsAndOffsetSettings);
        }
    }
}