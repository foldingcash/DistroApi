namespace StatsDownload.Core.Tests
{
    using System;
    using System.Linq;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestGoogleUsersFilter
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
                                        new UserData("user", 0, 0, 0),
                                        new UserData("GOOGLE", 0, 0, 0),
                                        new UserData("Google", 0, 0, 0),
                                        new UserData("google", 0, 0, 0),
                                        new UserData("google123456", 0, 0, 0)
                                    }, new[] { new FailedUserData() }));

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual.UsersData.Count(), Is.EqualTo(2));
            Assert.That(
                actual.UsersData.Count(
                    data => data.Name?.StartsWith("google", StringComparison.OrdinalIgnoreCase) ?? false), Is.EqualTo(0));
        }

        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            systemUnderTest = new GoogleUsersFilter(innerServiceMock);
        }
    }
}