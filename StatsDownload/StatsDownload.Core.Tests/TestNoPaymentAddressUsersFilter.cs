using System.Linq;
using NSubstitute;
using NUnit.Framework;
using StatsDownload.Core.Implementations.Tested;
using StatsDownload.Core.Interfaces;
using StatsDownload.Core.Interfaces.DataTransfer;

namespace StatsDownload.Core.Tests
{
    [TestFixture]
    public class TestNoPaymentAddressUsersFilter
    {
        [SetUp]
        public void SetUp()
        {
            innerServiceMock = Substitute.For<IStatsFileParserService>();

            settingsMock = Substitute.For<INoPaymentAddressUsersFilterSettings>();

            systemUnderTest = new NoPaymentAddressUsersFilter(innerServiceMock, settingsMock);
        }

        private IStatsFileParserService innerServiceMock;

        private IStatsFileParserService systemUnderTest;

        private INoPaymentAddressUsersFilterSettings settingsMock;

        [Test]
        public void Parse_WhenDisabled_DoesNotModifyResults()
        {
            settingsMock.Enabled.Returns(false);

            var expected = new ParseResults(null, null);
            innerServiceMock.Parse("fileData").Returns(expected);

            var actual = systemUnderTest.Parse("fileData");

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        public void Parse_WhenInvoked_FiltersResults()
        {
            settingsMock.Enabled.Returns(true);

            innerServiceMock.Parse("fileData")
                            .Returns(new ParseResults(
                                new[] {new UserData(), new UserData {BitcoinAddress = "addy"}},
                                new[] {new FailedUserData()}));

            ParseResults actual = systemUnderTest.Parse("fileData");

            Assert.That(actual.UsersData.Count(), Is.EqualTo(1));
            Assert.That(actual.UsersData.Count(data => string.IsNullOrWhiteSpace(data.BitcoinAddress)), Is.EqualTo(0));
        }
    }
}