namespace StatsDownload.Parsing.Tests
{
    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;

    [TestFixture]
    public class TestBitcoinCashAddressValidatorProvider
    {
        [SetUp]
        public void Setup()
        {
            logger = Substitute.For<ILogger<BitcoinCashAddressValidatorProvider>>();

            systemUnderTest = new BitcoinCashAddressValidatorProvider(logger);
        }

        private ILogger<BitcoinCashAddressValidatorProvider> logger;

        private IBitcoinCashAddressValidatorService systemUnderTest;

        [TestCase("bitcoincash:invalid")]
        [TestCase("invalid")]
        [TestCase("bitcoincash:")]
        [TestCase("bitcoincash")]
        [TestCase("1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i")]
        [TestCase("1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62X")]
        public void IsValidBitcoinCashAddress_WhenInvokedWithInvalidAddress_ReturnsFalse(string bitcoinCashAddress)
        {
            bool actual = systemUnderTest.IsValidBitcoinCashAddress(bitcoinCashAddress);

            Assert.IsFalse(actual);
        }

        [TestCase("bitcoincash:qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqfnhks603")]
        [TestCase("bitcoincash:qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqyhlgn0k34")]
        [TestCase("bitcoincash:qpm2qsznhks23z7629mms6s4cwef74vcwvy22gdx6a")]
        [TestCase("bitcoincash:qr95sy3j9xwd2ap32xkykttr4cvcu7as4y0qverfuy")]
        [TestCase("bitcoincash:qqq3728yw0y47sqn6l2na30mcw6zm78dzqre909m2r")]
        [TestCase("bitcoincash:ppm2qsznhks23z7629mms6s4cwef74vcwvn0h829pq")]
        [TestCase("bitcoincash:pr95sy3j9xwd2ap32xkykttr4cvcu7as4yc93ky28e")]
        [TestCase("bitcoincash:pqq3728yw0y47sqn6l2na30mcw6zm78dzq5ucqzc37")]
        [TestCase("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqfnhks603")]
        [TestCase("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqyhlgn0k34")]
        [TestCase("qpm2qsznhks23z7629mms6s4cwef74vcwvy22gdx6a")]
        [TestCase("qr95sy3j9xwd2ap32xkykttr4cvcu7as4y0qverfuy")]
        [TestCase("qqq3728yw0y47sqn6l2na30mcw6zm78dzqre909m2r")]
        [TestCase("ppm2qsznhks23z7629mms6s4cwef74vcwvn0h829pq")]
        [TestCase("pr95sy3j9xwd2ap32xkykttr4cvcu7as4yc93ky28e")]
        [TestCase("pqq3728yw0y47sqn6l2na30mcw6zm78dzq5ucqzc37")]
        public void IsValidBitcoinCashAddress_WhenInvokedWithValidAddress_ReturnsTrue(string bitcoinCashAddress)
        {
            bool actual = systemUnderTest.IsValidBitcoinCashAddress(bitcoinCashAddress);

            Assert.IsTrue(actual);
        }
    }
}