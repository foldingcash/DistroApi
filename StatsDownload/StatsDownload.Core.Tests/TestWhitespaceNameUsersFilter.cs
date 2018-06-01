namespace StatsDownload.Core.Tests
{
    using System.Linq;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestWhitespaceNameUsersFilter
    {
        private IStatsFileParserService innerServiceMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Parse_WhenInvoked_FiltersResults()
        {
            innerServiceMock.Parse("fileData")
                            .Returns(
                                new ParseResults(
                                    new[]
                                    {
                                        new UserData(),
                                        new UserData("", 0, 0, 0),
                                        new UserData("\t", 0, 0, 0),
                                        new UserData("name", 0, 0, 0)
                                    }, new[] { new FailedUserData() }));

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual.UsersData.Count(), Is.EqualTo(1));
            Assert.That(actual.UsersData.Count(data => string.IsNullOrWhiteSpace(data.Name)), Is.EqualTo(0));
        }

        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            systemUnderTest = new WhitespaceNameUsersFilter(innerServiceMock);
        }
    }
}