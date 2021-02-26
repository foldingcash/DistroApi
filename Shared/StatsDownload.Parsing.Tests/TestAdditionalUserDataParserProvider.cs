namespace StatsDownload.Parsing.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    [TestFixture]
    public class TestAdditionalUserDataParserProvider
    {
        [SetUp]
        public void SetUp()
        {
            bitcoinAddressValidatorServiceMock = Substitute.For<IBitcoinAddressValidatorService>();
            bitcoinAddressValidatorServiceMock.IsValidBitcoinAddress("Address").Returns(true);

            bitcoinCashAddressValidatorServiceMock = Substitute.For<IBitcoinCashAddressValidatorService>();
            bitcoinCashAddressValidatorServiceMock.IsValidBitcoinCashAddress("CashAddress").Returns(true);

            systemUnderTest = NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock,
                bitcoinCashAddressValidatorServiceMock);
        }

        private IBitcoinAddressValidatorService bitcoinAddressValidatorServiceMock;

        private IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorServiceMock;

        private IAdditionalUserDataParserService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(null, bitcoinCashAddressValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock, null));
        }

        [TestCase("Address")]
        [TestCase("Name_Address")]
        [TestCase("Name_TAG_Address")]
        [TestCase("Name__Address")]
        public void Parse_WhenInvoked_ReturnsBitcoinAddress(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.BitcoinAddress, Is.EqualTo("Address"));
        }

        [TestCase("CashAddress")]
        [TestCase("Name_CashAddress")]
        [TestCase("Name_TAG_CashAddress")]
        [TestCase("Name__CashAddress")]
        public void Parse_WhenInvoked_ReturnsBitcoinCashAddress(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.BitcoinCashAddress, Is.EqualTo("CashAddress"));
        }

        [TestCase("Name.Name_Address", "Name.Name")]
        [TestCase("Name-Name_TAG_Address", "Name-Name")]
        [TestCase("Name-Name_TAG_CashAddress", "Name-Name")]
        public void Parse_WhenInvoked_ReturnsComplexFriendlyName(string name, string expectedFriendlyName)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.FriendlyName, Is.EqualTo(expectedFriendlyName));
        }

        [TestCase("Name_Address")]
        [TestCase("Name_TAG_Address")]
        [TestCase("Name__Address")]
        [TestCase("Name__CashAddress")]
        public void Parse_WhenInvoked_ReturnsFriendlyName(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.FriendlyName, Is.EqualTo("Name"));
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("part1_part2_part3_part4")]
        public void Parse_WhenNoAdditionalUserData_ReturnsUserData(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.IsNull(userData.BitcoinAddress);
            Assert.IsNull(userData.FriendlyName);
        }

        [TestCase("name")]
        [TestCase("Name_BadAddress")]
        [TestCase("Name_TAG_BadAddress")]
        [TestCase("Name_TAG_BadCashAddress")]
        public void Parse_WhenNoBitcoinAddress_ReturnsNoAdditionalData(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.IsNull(userData.BitcoinAddress);
            Assert.IsNull(userData.FriendlyName);
        }

        [TestCase("name")]
        [TestCase("Address")]
        [TestCase("CashAddress")]
        public void Parse_WhenNoFriendlyName_ReturnsNoFriendlyName(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.IsNull(userData.FriendlyName);
        }

        private IAdditionalUserDataParserService NewAdditionalUserDataParserProvider(
            IBitcoinAddressValidatorService bitcoinAddressValidatorService,
            IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService)
        {
            return new AdditionalUserDataParserProvider(bitcoinAddressValidatorService,
                bitcoinCashAddressValidatorService);
        }
    }
}