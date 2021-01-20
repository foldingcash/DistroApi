namespace StatsDownload.Parsing.Tests
{
    using System;
    using System.Linq;

    using Microsoft.Extensions.Options;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Parsing.Filters;

    [TestFixture]
    public class TestNoPaymentAddressUsersFilter
    {
        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            filterSettings = new FilterSettings();

            filterSettingsOptionsMock = Substitute.For<IOptions<FilterSettings>>();
            filterSettingsOptionsMock.Value.Returns(filterSettings);

            systemUnderTest = new NoPaymentAddressUsersFilter(innerServiceMock, filterSettingsOptionsMock);

            downloadDateTime = DateTime.UtcNow;
        }

        private DateTime downloadDateTime;

        private readonly FilePayload FilePayload = new FilePayload { DecompressedDownloadFileData = "fileData" };

        private FilterSettings filterSettings;

        private IOptions<FilterSettings> filterSettingsOptionsMock;

        private IStatsFileParserService innerServiceMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Parse_WhenDisabled_DoesNotModifyResults()
        {
            filterSettings.NoPaymentAddressUsersEnabled = false;

            var expected = new ParseResults(downloadDateTime, null, null);
            innerServiceMock.Parse(FilePayload).Returns(expected);

            ParseResults actual = systemUnderTest.Parse(FilePayload);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Parse_WhenInvoked_FiltersResults()
        {
            filterSettings.NoPaymentAddressUsersEnabled = true;

            innerServiceMock.Parse(FilePayload).Returns(new ParseResults(downloadDateTime,
                new[] { new UserData(), new UserData { BitcoinAddress = "addy" } }, new[] { new FailedUserData() }));

            ParseResults actual = systemUnderTest.Parse(FilePayload);

            Assert.That(actual.UsersData.Count(), Is.EqualTo(1));
            Assert.That(actual.UsersData.Count(data => string.IsNullOrWhiteSpace(data.BitcoinAddress)), Is.EqualTo(0));
        }
    }
}