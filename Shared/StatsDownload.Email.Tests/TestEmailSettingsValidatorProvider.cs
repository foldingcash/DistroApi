namespace StatsDownload.Email.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class TestEmailSettingsValidatorProvider
    {
        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new EmailSettingsValidatorProvider();
        }

        private IEmailSettingsValidatorService systemUnderTest;

        [TestCase("")]
        [TestCase(null)]
        public void ParseFromAddress_WhenInvalidFromAddress_ThrowsEmailArgumentException(string unsafeFromAddress)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParseFromAddress(unsafeFromAddress));
        }

        [TestCase("user@domain.com")]
        public void ParseFromAddress_WhenValidFromAddress_ReturnsFromAddress(string unsafeFromAddress)
        {
            string actual = systemUnderTest.ParseFromAddress(unsafeFromAddress);

            Assert.That(actual, Is.EqualTo(unsafeFromAddress));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ParseFromDisplayName_WhenInvalidFromDisplayName_ThrowsEmailArgumentException(
            string unsafeFromDisplayName)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParseFromDisplayName(unsafeFromDisplayName));
        }

        [TestCase("Display Name")]
        public void ParseFromDisplayName_WhenValidFromDisplayName_ReturnsFromDisplayName(string unsafeFromDisplayName)
        {
            string actual = systemUnderTest.ParseFromDisplayName(unsafeFromDisplayName);

            Assert.That(actual, Is.EqualTo(unsafeFromDisplayName));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ParsePassword_WhenInvalidPassword_ThrowsEmailArgumentException(string unsafePassword)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParsePassword(unsafePassword));
        }

        [TestCase("password")]
        public void ParsePassword_WhenValidPassword_ReturnsPassword(string unsafePassword)
        {
            string actual = systemUnderTest.ParsePassword(unsafePassword);

            Assert.That(actual, Is.EqualTo(unsafePassword));
        }

        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(65536)]
        public void ParsePort_WhenInvokedWithInvalidIntPort_ThrowsEmailArgumentException(int unsafePort)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParsePort(unsafePort));
        }

        [TestCase(1, 1)]
        [TestCase(65535, 65535)]
        public void ParsePort_WhenInvokedWithValidPort_ReturnsParsedPort(int unsafePort, int expected)
        {
            int actual = systemUnderTest.ParsePort(unsafePort);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(null)]
        [TestCase("")]
        public void ParseReceivers_WhenInvokedWithInvalidReceiver_ThrowsEmailArgumentException(string unsafeReceivers)
        {
            Assert.Throws<EmailArgumentException>(() =>
                systemUnderTest.ParseReceivers(new[] { unsafeReceivers, unsafeReceivers }));
        }

        [TestCase(0)]
        [TestCase(1)]
        public void ParseReceivers_WhenInvokedWithNoReceiver_ThrowsEmailArgumentException(int arrayLength)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParseReceivers(new string[arrayLength]));
        }

        [Test]
        public void ParseReceivers_WhenInvokedWithNull_ThrowsEmailArgumentException()
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParseReceivers(null));
        }

        [Test]
        public void ParseReceivers_WhenInvokedWithOneReceiver_ReturnsReceiver()
        {
            IEnumerable<string> actual = systemUnderTest.ParseReceivers(new[] { "user@domain.tld" });

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(1));
            Assert.That(actual.First(), Is.EqualTo("user@domain.tld"));
        }

        [Test]
        public void ParseReceivers_WhenInvokedWithReceivers_ReturnsReceivers()
        {
            IEnumerable<string> actual = systemUnderTest.ParseReceivers(new[] { "user@domain.tld", "user@domain.tld" });

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(2));
            Assert.That(actual.First(), Is.EqualTo("user@domain.tld"));
        }

        [Test]
        public void ParseReceivers_WhenInvokedWithSomeEmptyReceivers_FiltersOutEmptyReceivers()
        {
            IEnumerable<string> actual = systemUnderTest.ParseReceivers(new[] { "", "user@domain.tld", null });

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(1));
            Assert.That(actual.First(), Is.EqualTo("user@domain.tld"));
        }

        [TestCase("")]
        [TestCase(null)]
        public void ParseSmtpHost_WhenInvalidSmtpHost_ThrowsEmailArgumentException(string unsafeSmtpHost)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParseSmtpHost(unsafeSmtpHost));
        }

        [TestCase("google.com")]
        public void ParseSmtpHost_WhenValidSmtpHost_ReturnsSmtpHost(string unsafeSmtpHost)
        {
            string actual = systemUnderTest.ParseSmtpHost(unsafeSmtpHost);

            Assert.That(actual, Is.EqualTo(unsafeSmtpHost));
        }
    }
}