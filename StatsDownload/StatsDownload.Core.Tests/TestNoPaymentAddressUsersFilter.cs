namespace StatsDownload.Core.Tests
{
    using System.Linq;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Implementations.Tested;
    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestNoPaymentAddressUsersFilter
    {
        private IStatsFileParserService innerServiceMock;

        private IStatsFileParserService systemUnderTest;

        [Test]
        public void Parse_WhenInvoked_FiltersResults()
        {
            innerServiceMock.Parse("fileData")
                            .Returns(new ParseResults(
                                new[] { new UserData(), new UserData { BitcoinAddress = "addy" } },
                                new[] { new FailedUserData() }));

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual.UsersData.Count(), Is.EqualTo(1));
            Assert.That(actual.UsersData.Count(data => string.IsNullOrWhiteSpace(data.BitcoinAddress)), Is.EqualTo(0));
        }

        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            systemUnderTest = new NoPaymentAddressUsersFilter(innerServiceMock);
        }
    }
}