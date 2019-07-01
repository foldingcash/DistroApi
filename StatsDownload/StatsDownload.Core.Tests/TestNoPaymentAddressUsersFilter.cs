namespace StatsDownload.Core.Tests
{
    using System;
    using System.Linq;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestNoPaymentAddressUsersFilter
    {
        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            settingsMock = Substitute.For<INoPaymentAddressUsersFilterSettings>();

            systemUnderTest = new NoPaymentAddressUsersFilter(innerServiceMock, settingsMock);

            downloadDateTime = DateTime.UtcNow;
        }

        private DateTime downloadDateTime;

        private IStatsFileParserService innerServiceMock;

        private INoPaymentAddressUsersFilterSettings settingsMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Parse_WhenDisabled_DoesNotModifyResults()
        {
            settingsMock.Enabled.Returns(false);

            var expected = new ParseResults(downloadDateTime, null, null);
            innerServiceMock.Parse("fileData").Returns(expected);

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Parse_WhenInvoked_FiltersResults()
        {
            settingsMock.Enabled.Returns(true);

            innerServiceMock.Parse("fileData").Returns(new ParseResults(downloadDateTime,
                new[] { new UserData(), new UserData { BitcoinAddress = "addy" } },
                new[] { new FailedUserData() }));

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual.UsersData.Count(), Is.EqualTo(1));
            Assert.That(actual.UsersData.Count(data => string.IsNullOrWhiteSpace(data.BitcoinAddress)), Is.EqualTo(0));
        }
    }
}