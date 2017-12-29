namespace StatsDownload.Core.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestDownloadSettingsValidatorProvider
    {
        private static readonly string[] BadAcceptAnySslCertValues = { "anything else" };

        private static readonly string[] BadTimeoutValues = { "-1", "-2", "0", "99", "3601" };

        private static readonly string[] FalseAcceptAnySslCertValues = { "false", "FALSE" };

        private static readonly string[] GoodTimeoutValues = { "100", "3600" };

        private static readonly string[] TrueAcceptAnySslCertValues = { "true", "TRUE" };

        private IDownloadSettingsValidatorService systemUnderTest;

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new DownloadSettingsValidatorProvider();
        }

        [TestCaseSource(nameof(BadAcceptAnySslCertValues))]
        public void TryParseAcceptAnySslCert_WhenBadValueIsProvided_ReturnsFalse(string input)
        {
            bool output;
            bool actual = systemUnderTest.TryParseAcceptAnySslCert(input, out output);

            Assert.That(actual, Is.False);
        }

        [TestCaseSource(nameof(BadAcceptAnySslCertValues))]
        public void TryParseAcceptAnySslCert_WhenBadValueIsProvided_ReturnsParsedFalse(string input)
        {
            bool actual;
            systemUnderTest.TryParseAcceptAnySslCert(input, out actual);

            Assert.That(actual, Is.False);
        }

        [TestCaseSource(nameof(FalseAcceptAnySslCertValues))]
        public void TryParseAcceptAnySslCert_WhenFalseIsProvided_ReturnsParsedFalse(string input)
        {
            bool actual;
            systemUnderTest.TryParseAcceptAnySslCert(input, out actual);

            Assert.That(actual, Is.False);
        }

        [TestCaseSource(nameof(FalseAcceptAnySslCertValues))]
        public void TryParseAcceptAnySslCert_WhenFalseIsProvided_ReturnsTrue(string input)
        {
            bool output;
            bool actual = systemUnderTest.TryParseAcceptAnySslCert(input, out output);

            Assert.That(actual, Is.True);
        }

        [TestCaseSource(nameof(TrueAcceptAnySslCertValues))]
        public void TryParseAcceptAnySslCert_WhenTrueIsProvided_ReturnsParsedTrue(string input)
        {
            bool actual;
            systemUnderTest.TryParseAcceptAnySslCert(input, out actual);

            Assert.That(actual, Is.True);
        }

        [TestCaseSource(nameof(TrueAcceptAnySslCertValues))]
        public void TryParseAcceptAnySslCert_WhenTrueIsProvided_ReturnsTrue(string input)
        {
            bool output;
            bool actual = systemUnderTest.TryParseAcceptAnySslCert(input, out output);

            Assert.That(actual, Is.True);
        }

        [TestCaseSource(nameof(BadTimeoutValues))]
        public void TryParseTimeout_WhenAnIntIsNotProvided_ReturnsFalse(string input)
        {
            int output;
            bool actual = systemUnderTest.TryParseTimeout(input, out output);

            Assert.That(actual, Is.False);
        }

        [TestCase("100", 100)]
        public void TryParseTimeout_WhenAnIntIsProvided_ReturnsTheParsedInt(string input, int expected)
        {
            int actual;
            systemUnderTest.TryParseTimeout(input, out actual);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCaseSource(nameof(BadTimeoutValues))]
        public void TryParseTimeout_WhenAnIntNotInRangeIsProvided_ReturnsFalse(string input)
        {
            int output;
            bool actual = systemUnderTest.TryParseTimeout(input, out output);

            Assert.That(actual, Is.False);
        }

        [TestCaseSource(nameof(GoodTimeoutValues))]
        public void TryParseTimeout_WhenAnIntWithingRangeIsProvided_ReturnsTrue(string input)
        {
            int output;
            bool actual = systemUnderTest.TryParseTimeout(input, out output);

            Assert.That(actual, Is.True);
        }
    }
}