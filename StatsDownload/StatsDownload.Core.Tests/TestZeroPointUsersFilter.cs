namespace StatsDownload.Core.Tests
{
    using System;
    using System.Linq;
    using Implementations;
    using Interfaces;
    using Interfaces.DataTransfer;
    using NSubstitute;
    using NUnit.Framework;

    [TestFixture]
    public class TestZeroPointUsersFilter
    {
        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            settingsMock = Substitute.For<IZeroPointUsersFilterSettings>();

            systemUnderTest = new ZeroPointUsersFilter(innerServiceMock, settingsMock);

            downloadDateTime = DateTime.UtcNow;
        }

        private DateTime downloadDateTime;

        private IStatsFileParserService innerServiceMock;

        private IZeroPointUsersFilterSettings settingsMock;

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

            innerServiceMock.Parse("fileData")
                            .Returns(new ParseResults(downloadDateTime,
                                new[] { new UserData(), new UserData(0, null, 1, 0, 0) },
                                new[] { new FailedUserData() }));

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual.UsersData.Count(data => data.TotalPoints == 0), Is.EqualTo(0));
        }
    }
}