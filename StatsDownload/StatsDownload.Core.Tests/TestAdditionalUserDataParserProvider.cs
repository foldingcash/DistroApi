namespace StatsDownload.Core.Tests
{
    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestAdditionalUserDataParserProvider
    {
        private IBitcoinAddressValidatorService bitcoinAddressValidatorServiceMock;

        private IAdditionalUserDataParserService systemUnderTest;

        [TestCase("Address")]
        [TestCase("Name_Address")]
        [TestCase("Name.Address")]
        [TestCase("Name-Address")]
        [TestCase("Name_TAG_Address")]
        [TestCase("Name.TAG.Address")]
        [TestCase("Name-TAG-Address")]
        public void Parse_WhenInvoked_ReturnsBitcoinAddress(string name)
        {
            var userData = new UserData(name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.BitcoinAddress, Is.EqualTo("Address"));
        }

        [TestCase("Name_Address")]
        [TestCase("Name.Address")]
        [TestCase("Name-Address")]
        [TestCase("Name_TAG_Address")]
        [TestCase("Name.TAG.Address")]
        [TestCase("Name-TAG-Address")]
        public void Parse_WhenInvoked_ReturnsFriendlyName(string name)
        {
            var userData = new UserData(name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.FriendlyName, Is.EqualTo("Name"));
        }

        [TestCase("name")]
        [TestCase("Name_BadAddress")]
        [TestCase("Name.BadAddress")]
        [TestCase("Name-BadAddress")]
        [TestCase("Name_TAG_BadAddress")]
        [TestCase("Name.TAG.BadAddress")]
        [TestCase("Name-TAG-BadAddress")]
        public void Parse_WhenNoBitcoinAddress_ReturnsNoBitcoinAddress(string name)
        {
            var userData = new UserData(name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.IsNull(userData.BitcoinAddress);
        }

        [TestCase("name")]
        [TestCase("Address")]
        public void Parse_WhenNoFriendlyName_ReturnsNoFriendlyName(string name)
        {
            var userData = new UserData(name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.IsNull(userData.FriendlyName);
        }

        [SetUp]
        public void SetUp()
        {
            bitcoinAddressValidatorServiceMock = Substitute.For<IBitcoinAddressValidatorService>();
            bitcoinAddressValidatorServiceMock.IsValidBitcoinAddress("Address").Returns(true);

            systemUnderTest = new AdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock);
        }
    }
}