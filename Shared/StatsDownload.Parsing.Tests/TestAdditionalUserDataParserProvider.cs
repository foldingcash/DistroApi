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
            bitcoinCashAddressValidatorServiceMock.GetBitcoinAddress("CashAddress").Returns("Address");

            slpAddressValidatorServiceMock = Substitute.For<ISlpAddressValidatorService>();
            slpAddressValidatorServiceMock.IsValidSlpAddress("SlpAddress").Returns(true);
            slpAddressValidatorServiceMock.GetBitcoinAddress("SlpAddress").Returns("Address");

            systemUnderTest = NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock,
                bitcoinCashAddressValidatorServiceMock, slpAddressValidatorServiceMock);
        }

        private IBitcoinAddressValidatorService bitcoinAddressValidatorServiceMock;

        private IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorServiceMock;

        private ISlpAddressValidatorService slpAddressValidatorServiceMock;

        private IAdditionalUserDataParserService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(null, bitcoinCashAddressValidatorServiceMock,
                    slpAddressValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock, null,
                    slpAddressValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock,
                    bitcoinCashAddressValidatorServiceMock, null));
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
            Assert.IsNull(userData.BitcoinCashAddress);
            Assert.IsNull(userData.SlpAddress);
        }

        [TestCase("Name.Name_Address", "Name.Name")]
        [TestCase("Name-Name_TAG_Address", "Name-Name")]
        [TestCase("Name-Name_TAG_CashAddress", "Name-Name")]
        [TestCase("Name-Name_TAG_SlpAddress", "Name-Name")]
        public void Parse_WhenInvoked_ReturnsComplexFriendlyName(string name, string expectedFriendlyName)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.FriendlyName, Is.EqualTo(expectedFriendlyName));
        }

        [TestCase("Name_Address")]
        [TestCase("Name_TAG_Address")]
        [TestCase("Name_TAG_CashAddress")]
        [TestCase("Name_TAG_SlpAddress")]
        [TestCase("Name__Address")]
        [TestCase("Name__CashAddress")]
        [TestCase("Name__SlpAddress")]
        public void Parse_WhenInvoked_ReturnsFriendlyName(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.FriendlyName, Is.EqualTo("Name"));
        }

        [TestCase("CashAddress")]
        [TestCase("Name_CashAddress")]
        [TestCase("Name_TAG_CashAddress")]
        [TestCase("Name__CashAddress")]
        public void Parse_WhenInvokedWithBitcoinCashAddress_ReturnsAddresses(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.BitcoinCashAddress, Is.EqualTo("CashAddress"));
            Assert.That(userData.BitcoinAddress, Is.EqualTo("Address"));
            Assert.IsNull(userData.SlpAddress);
        }

        [TestCase("SlpAddress")]
        [TestCase("Name_SlpAddress")]
        [TestCase("Name_TAG_SlpAddress")]
        [TestCase("Name__SlpAddress")]
        public void Parse_WhenInvokedWithSlpAddress_ReturnsAddresses(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.SlpAddress, Is.EqualTo("SlpAddress"));
            Assert.That(userData.BitcoinAddress, Is.EqualTo("Address"));
            Assert.IsNull(userData.BitcoinCashAddress);
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
        [TestCase("Name_TAG_BadSlpAddress")]
        public void Parse_WhenNoBitcoinAddress_ReturnsNoAdditionalData(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.IsNull(userData.FriendlyName);
            Assert.IsNull(userData.BitcoinAddress);
            Assert.IsNull(userData.BitcoinCashAddress);
            Assert.IsNull(userData.SlpAddress);
        }

        [TestCase("name")]
        [TestCase("Address")]
        [TestCase("CashAddress")]
        [TestCase("SlpAddress")]
        public void Parse_WhenNoFriendlyName_ReturnsNoFriendlyName(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.IsNull(userData.FriendlyName);
        }

        private IAdditionalUserDataParserService NewAdditionalUserDataParserProvider(
            IBitcoinAddressValidatorService bitcoinAddressValidatorService,
            IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService,
            ISlpAddressValidatorService slpAddressValidatorService)
        {
            return new AdditionalUserDataParserProvider(bitcoinAddressValidatorService,
                bitcoinCashAddressValidatorService, slpAddressValidatorService);
        }
    }
}