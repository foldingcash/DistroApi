namespace StatsDownload.Core.Tests
{
    using System;

    using Microsoft.Extensions.Options;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Settings;
    using StatsDownload.Parsing;

    [TestFixture]
    public class TestStatsFileDateTimeFormatsAndOffsetProvider
    {
        [SetUp]
        public void SetUp()
        {
            dateTimeSettings = new DateTimeSettings
                               {
                                   Formats = new[]
                                             {
                                                 new DateTimeFormat { Format = "ZONE1", HourOffset = -1 },
                                                 new DateTimeFormat { Format = "ZONE2", HourOffset = 0 },
                                                 new DateTimeFormat { Format = "ZONE3", HourOffset = 1 }
                                             }
                               };

            dateTimeSettingsOptionsMock = Substitute.For<IOptions<DateTimeSettings>>();
            dateTimeSettingsOptionsMock.Value.Returns(dateTimeSettings);

            systemUnderTest = NewStatsFileDateTimeFormatsAndOffsetProvider(dateTimeSettingsOptionsMock);
        }

        private readonly (string format, int hourOffset)[] dateTimeFormatsAndOffset =
        {
            ("ddd MMM  d HH:mm:ss GMT yyyy", 0),
            ("ddd MMM dd HH:mm:ss GMT yyyy", 0),
            ("ddd MMM  d HH:mm:ss CDT yyyy", -5),
            ("ddd MMM dd HH:mm:ss CDT yyyy", -5),
            ("ddd MMM  d HH:mm:ss CST yyyy", -6),
            ("ddd MMM dd HH:mm:ss CST yyyy", -6),
            ("ddd MMM  d HH:mm:ss PDT yyyy", -7),
            ("ddd MMM dd HH:mm:ss PDT yyyy", -7),
            ("ddd MMM  d HH:mm:ss PST yyyy", -8),
            ("ddd MMM dd HH:mm:ss PST yyyy", -8)
        };

        private DateTimeSettings dateTimeSettings;

        private IOptions<DateTimeSettings> dateTimeSettingsOptionsMock;

        private IStatsFileDateTimeFormatsAndOffsetService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewStatsFileDateTimeFormatsAndOffsetProvider(null));
        }

        [Test]
        public void GetStatsFileDateTimeZoneAndOffset_WhenInvoked_ReturnsDateTimeFormatsAndOffsetsConstants()
        {
            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.IsSupersetOf(actual, dateTimeFormatsAndOffset);
        }

        [Test]
        public void GetStatsFileDateTimeZoneAndOffset_WhenSettingsEmpty_ReturnsDateTimeFormats()
        {
            dateTimeSettings.Formats = new DateTimeFormat[0];

            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.AreEquivalent(actual, dateTimeFormatsAndOffset);
        }

        [Test]
        public void GetStatsFileDateTimeZoneAndOffset_WhenSettingsMissing_ReturnsDateTimeFormats()
        {
            dateTimeSettings.Formats = null;

            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.AreEquivalent(actual, dateTimeFormatsAndOffset);
        }

        [Test]
        public void GetStatsFileDateTimeZoneAndOffsetSettings_WhenInvoked_ReturnsConfiguredDateTimeFormatsAndOffsets()
        {
            (string format, int hourOffset)[] actual = systemUnderTest.GetStatsFileDateTimeFormatsAndOffset();

            CollectionAssert.IsSupersetOf(actual,
                new[]
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
            IOptions<DateTimeSettings> dateTimeSettingsOptions)
        {
            return new StatsFileDateTimeFormatsAndOffsetProvider(dateTimeSettingsOptions);
        }
    }
}