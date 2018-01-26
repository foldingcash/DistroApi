namespace StatsDownload.Email.Tests
{
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    [TestFixture]
    public class TestEmailSettingsValidatorProvider
    {
        private IEmailSettingsValidatorService systemUnderTest;

        [TestCase("-1")]
        [TestCase("0")]
        [TestCase("65536")]
        public void ParsePort_WhenInvokedWithInvalidIntPort_ThrowsEmailArgumentException(string unsafePort)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParsePort(unsafePort));
        }

        [TestCase("NaN")]
        [TestCase(null)]
        [TestCase("")]
        public void ParsePort_WhenInvokedWithInvalidPort_ThrowsEmailArgumentException(string unsafePort)
        {
            Assert.Throws<EmailArgumentException>(() => systemUnderTest.ParsePort(unsafePort));
        }

        [TestCase("1", 1)]
        [TestCase("65535", 65535)]
        public void ParsePort_WhenInvokedWithValidPort_ReturnsParsedPort(string unsafePort, int expected)
        {
            int actual = systemUnderTest.ParsePort(unsafePort);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(null)]
        [TestCase("")]
        public void ParseReceivers_WhenInvokedWithInvalidReceiver_ThrowsEmailArgumentException(string unsafeReceivers)
        {
            Assert.Throws<EmailArgumentException>((() => systemUnderTest.ParseReceivers(unsafeReceivers)));
        }

        [TestCase("user@domain.tld")]
        [TestCase("user@domain.tld;")]
        public void ParseReceivers_WhenInvokedWithOneReceiver_ReturnsReceiver(string unsafeReceivers)
        {
            IEnumerable<string> actual = systemUnderTest.ParseReceivers(unsafeReceivers);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(1));
            Assert.That(actual.First(), Is.EqualTo("user@domain.tld"));
        }

        [Test]
        public void ParseReceivers_WhenInvokedWithReceivers_ReturnsReceivers()
        {
            IEnumerable<string> actual = systemUnderTest.ParseReceivers("user@domain.tld;user@domain.tld;");

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.Count(), Is.EqualTo(2));
            Assert.That(actual.First(), Is.EqualTo("user@domain.tld"));
        }

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new EmailSettingsValidatorProvider();
        }
    }
}