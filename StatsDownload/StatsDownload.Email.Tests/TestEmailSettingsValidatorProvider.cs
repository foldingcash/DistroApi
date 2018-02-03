namespace StatsDownload.Email.Tests
{
    using System;
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
        public void ParsePort_WhenInvokedWithInvalidIntPort_ThrowsArgumentOutOfRangeException(string unsafePort)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => systemUnderTest.ParsePort(unsafePort));
        }

        [TestCase("NaN")]
        public void ParsePort_WhenInvokedWithInvalidPort_ThrowsArgumentException(string unsafePort)
        {
            Assert.Throws<ArgumentException>(() => systemUnderTest.ParsePort(unsafePort));
        }

        [TestCase("1", 1)]
        [TestCase("65535", 65535)]
        public void ParsePort_WhenInvokedWithValidPort_ReturnsParsedPort(string unsafePort, int expected)
        {
            int actual = systemUnderTest.ParsePort(unsafePort);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase("user@domain.tld")]
        [TestCase("user@domain.tld;")]
        public void ParseReceivers_WhenInvokedWithOneReceiver_ReturnsReceiver(string receiver)
        {
            IEnumerable<string> actual = systemUnderTest.ParseReceivers(receiver);

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