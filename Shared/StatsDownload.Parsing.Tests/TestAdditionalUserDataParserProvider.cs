namespace StatsDownload.Parsing.Tests
{
    using System;
    using Core.Interfaces;
    using Core.Interfaces.DataTransfer;
    using NSubstitute;
    using NUnit.Framework;

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

            cashTokensAddressValidatorServiceMock = Substitute.For<ICashTokensAddressValidatorService>();
            cashTokensAddressValidatorServiceMock.IsValidCashTokensAddress("CashTokens").Returns(true);
            cashTokensAddressValidatorServiceMock.GetBitcoinAddress("CashTokens").Returns("Address");

            systemUnderTest = NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock,
                bitcoinCashAddressValidatorServiceMock, slpAddressValidatorServiceMock,
                cashTokensAddressValidatorServiceMock);
        }

        private IBitcoinAddressValidatorService bitcoinAddressValidatorServiceMock;

        private IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorServiceMock;

        private ICashTokensAddressValidatorService cashTokensAddressValidatorServiceMock;

        private ISlpAddressValidatorService slpAddressValidatorServiceMock;

        private IAdditionalUserDataParserService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(null, bitcoinCashAddressValidatorServiceMock,
                    slpAddressValidatorServiceMock, cashTokensAddressValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock, null,
                    slpAddressValidatorServiceMock, cashTokensAddressValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock,
                    bitcoinCashAddressValidatorServiceMock, null, cashTokensAddressValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(() =>
                NewAdditionalUserDataParserProvider(bitcoinAddressValidatorServiceMock,
                    bitcoinCashAddressValidatorServiceMock, slpAddressValidatorServiceMock, null));
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
            Assert.That(userData.BitcoinCashAddress, Is.Null);
            Assert.That(userData.SlpAddress, Is.Null);
            Assert.That(userData.CashTokensAddress, Is.Null);
        }

        [TestCase("Name.Name_Address", "Name.Name")]
        [TestCase("Name-Name_TAG_Address", "Name-Name")]
        [TestCase("Name-Name_TAG_CashAddress", "Name-Name")]
        [TestCase("Name-Name_TAG_SlpAddress", "Name-Name")]
        [TestCase("Name-Name_TAG_CashTokens", "Name-Name")]
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
        [TestCase("Name_TAG_CashTokens")]
        [TestCase("Name__Address")]
        [TestCase("Name__CashAddress")]
        [TestCase("Name__SlpAddress")]
        [TestCase("Name__CashTokens")]
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
            Assert.That(userData.SlpAddress, Is.Null);
            Assert.That(userData.CashTokensAddress, Is.Null);
        }

        [TestCase("CashTokens")]
        [TestCase("Name_CashTokens")]
        [TestCase("Name_TAG_CashTokens")]
        [TestCase("Name__CashTokens")]
        public void Parse_WhenInvokedWithCashTokensAddress_ReturnsAddresses(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.CashTokensAddress, Is.EqualTo("CashTokens"));
            Assert.That(userData.BitcoinAddress, Is.EqualTo("Address"));
            Assert.That(userData.SlpAddress, Is.Null);
            Assert.That(userData.BitcoinCashAddress, Is.Null);
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
            Assert.That(userData.BitcoinCashAddress, Is.Null);
            Assert.That(userData.CashTokensAddress, Is.Null);
        }

        [TestCase("")]
        [TestCase(null)]
        [TestCase("part1_part2_part3_part4")]
        public void Parse_WhenNoAdditionalUserData_ReturnsUserData(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.BitcoinAddress, Is.Null);
            Assert.That(userData.FriendlyName, Is.Null);
        }

        [TestCase("name")]
        [TestCase("Name_BadAddress")]
        [TestCase("Name_TAG_BadAddress")]
        [TestCase("Name_TAG_BadCashAddress")]
        [TestCase("Name_TAG_BadSlpAddress")]
        [TestCase("Name_TAG_BadCashTokens")]
        public void Parse_WhenNoBitcoinAddress_ReturnsNoAdditionalData(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.FriendlyName, Is.Null);
            Assert.That(userData.BitcoinAddress, Is.Null);
            Assert.That(userData.BitcoinCashAddress, Is.Null);
            Assert.That(userData.SlpAddress, Is.Null);
            Assert.That(userData.CashTokensAddress, Is.Null);
        }

        [TestCase("name")]
        [TestCase("Address")]
        [TestCase("CashAddress")]
        [TestCase("SlpAddress")]
        [TestCase("CashTokens")]
        public void Parse_WhenNoFriendlyName_ReturnsNoFriendlyName(string name)
        {
            var userData = new UserData(0, name, 0, 0, 0);

            systemUnderTest.Parse(userData);

            Assert.That(userData.FriendlyName, Is.Null);
        }

        private IAdditionalUserDataParserService NewAdditionalUserDataParserProvider(
            IBitcoinAddressValidatorService bitcoinAddressValidatorService,
            IBitcoinCashAddressValidatorService bitcoinCashAddressValidatorService,
            ISlpAddressValidatorService slpAddressValidatorService,
            ICashTokensAddressValidatorService cashTokensAddressValidatorService)
        {
            return new AdditionalUserDataParserProvider(bitcoinAddressValidatorService,
                bitcoinCashAddressValidatorService, slpAddressValidatorService, cashTokensAddressValidatorService);
        }
    }
}