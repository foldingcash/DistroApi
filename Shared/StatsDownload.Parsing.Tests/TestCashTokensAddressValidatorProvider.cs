namespace StatsDownload.Parsing.Tests
{
    using Microsoft.Extensions.Logging;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;

    [TestFixture]
    public class TestCashTokensAddressValidatorProvider
    {
        [SetUp]
        public void SetUp()
        {
            logger = Substitute.For<ILogger<CashTokensAddressValidatorProvider>>();

            systemUnderTest = new CashTokensAddressValidatorProvider(logger);
        }

        private ILogger<CashTokensAddressValidatorProvider> logger;

        private ICashTokensAddressValidatorService systemUnderTest;

        [TestCase("bitcoincash:zqq3728yw0y47sqn6l2na30mcw6zm78dzqynk3ta4s", "16w1D5WRVKJuZUsSRzdLp9w3YGcgoxDXb")]
        [TestCase("bitcoincash:rpm2qsznhks23z7629mms6s4cwef74vcwv59yeyr7n", "3CWFddi6m4ndiGyKqzYvsFYagqDLPVMTzC")]
        [TestCase("zqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqweyg7usz", "1111111111111111111114oLvT2")]
        [TestCase("zqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqys4mdpswx", "11111111111111111111BZbvjr")]
        [TestCase("zpm2qsznhks23z7629mms6s4cwef74vcwvrqekrq9w", "1BpEi6DfDAUFd7GtittLSdBeYJvcoaVggu")]
        [TestCase("zr95sy3j9xwd2ap32xkykttr4cvcu7as4yg2l8d0rh", "1KXrWXciRDZUpQwQmuM1DbwsKDLYAYsVLR")]
        [TestCase("zqq3728yw0y47sqn6l2na30mcw6zm78dzqynk3ta4s", "16w1D5WRVKJuZUsSRzdLp9w3YGcgoxDXb")]
        [TestCase("rpm2qsznhks23z7629mms6s4cwef74vcwv59yeyr7n", "3CWFddi6m4ndiGyKqzYvsFYagqDLPVMTzC")]
        [TestCase("bitcoincash:rr95sy3j9xwd2ap32xkykttr4cvcu7as4yl0zg2vc2", "3LDsS579y7sruadqu11beEJoTjdFiFCdX4")]
        [TestCase("rqq3728yw0y47sqn6l2na30mcw6zm78dzqnkt7v7wd", "31nwvkZwyPdgzjBJZXfDmSWsC4ZLKpYyUw")]
        public void GetBitcoinAddress_WhenInvoked_ReturnsBitcoinAddress(string address, string expected)
        {
            string actual = systemUnderTest.GetBitcoinAddress(address);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("bitcoincash:invalid")]
        [TestCase("invalid")]
        [TestCase("bitcoincash:")]
        [TestCase("bitcoincash")]
        [TestCase("1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62i")]
        [TestCase("1AGNa15ZQXAZUgFiqJ2i7Z2DPU2J6hW62X")]
        [TestCase("simpleledger:qpm2qsznhks23z7629mms6s4cwef74vcwvy22gdx6a")]
        [TestCase("simpleledger:pqq3728yw0y47sqn6l2na30mcw6zm78dzq5ucqzc37")]
        [TestCase("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqfnhks603")]
        [TestCase("qqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqqyhlgn0k34")]
        [TestCase("qpm2qsznhks23z7629mms6s4cwef74vcwvy22gdx6a")]
        [TestCase("qr95sy3j9xwd2ap32xkykttr4cvcu7as4y0qverfuy")]
        [TestCase("qqq3728yw0y47sqn6l2na30mcw6zm78dzqre909m2r")]
        [TestCase("ppm2qsznhks23z7629mms6s4cwef74vcwvn0h829pq")]
        [TestCase("pr95sy3j9xwd2ap32xkykttr4cvcu7as4yc93ky28e")]
        [TestCase("pqq3728yw0y47sqn6l2na30mcw6zm78dzq5ucqzc37")]
        [TestCase("zqydwz7znetl8ya46s223q4afrpv9gy2mgyqhfu2dm")]
        [TestCase("bchtest:zqydwz7znetl8ya46s223q4afrpv9gy2mgyqhfu2dm")]
        [TestCase("zrfh6hz9g782fd48uryk0ras7j6laxncfqtc88plnd")]
        public void IsValidCashTokensAddress_WhenInvokedWithInvalidAddress_ReturnsFalse(string address)
        {
            bool actual = systemUnderTest.IsValidCashTokensAddress(address);

            Assert.That(actual, Is.False);
        }

        [TestCase("bitcoincash:zqydwz7znetl8ya46s223q4afrpv9gy2mgqjnw7a28")]
        [TestCase("zqydwz7znetl8ya46s223q4afrpv9gy2mgqjnw7a28")]
        [TestCase("bitcoincash:zqmkcwn7sxkqaf2u2ah2fgz8nc3cyekz5gh938ycua")]
        [TestCase("zqmkcwn7sxkqaf2u2ah2fgz8nc3cyekz5gh938ycua")]
        [TestCase("bitcoincash:rr95sy3j9xwd2ap32xkykttr4cvcu7as4yl0zg2vc2")]
        [TestCase("rr95sy3j9xwd2ap32xkykttr4cvcu7as4yl0zg2vc2")]
        public void IsValidCashTokensAddress_WhenInvokedWithValidAddress_ReturnsTrue(string address)
        {
            bool actual = systemUnderTest.IsValidCashTokensAddress(address);

            Assert.That(actual, Is.True);
        }
    }
}