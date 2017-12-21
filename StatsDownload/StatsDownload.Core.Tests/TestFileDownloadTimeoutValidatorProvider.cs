namespace StatsDownload.Core.Tests
{
    using NUnit.Framework;

    [TestFixture]
    public class TestFileDownloadTimeoutValidatorProvider
    {
        private static readonly string[] BadTimeoutValues = { "-1", "-2", "0", "99", "3601" };

        private static readonly string[] GoodTimeoutValues = { "100", "3600" };

        private IFileDownloadTimeoutValidatorService systemUnderTest;

        [SetUp]
        public void SetUp()
        {
            systemUnderTest = new FileDownloadTimeoutValidatorProvider();
        }

        [TestCaseSource(nameof(BadTimeoutValues))]
        public void TryParseTimeout_WhenAnIntIsNotProvided_ReturnsFalse(string input)
        {
            int output;
            bool actual = systemUnderTest.TryParseTimeout(input, out output);

            Assert.That(actual, Is.False);
        }

        //[TestCase("0", 0)]
        //[TestCase("-1", 0)]
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