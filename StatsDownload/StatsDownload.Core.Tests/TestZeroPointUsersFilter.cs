namespace StatsDownload.Core.Tests
{
    using System.Linq;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestZeroPointUsersFilter
    {
        private IStatsFileParserService innerServiceMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Parse_WhenInvoked_FiltersResults()
        {
            innerServiceMock.Parse("fileData")
                            .Returns(new ParseResults(new[] { new UserData(), new UserData(null, 1, 0, 0) },
                                new[] { new FailedUserData() }));

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual.UsersData.Count(data => data.TotalPoints == 0), Is.EqualTo(0));
        }

        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            systemUnderTest = new ZeroPointUsersFilter(innerServiceMock);
        }
    }
}