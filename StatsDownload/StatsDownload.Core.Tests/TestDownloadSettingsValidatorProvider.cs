namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Parsing;

    [TestFixture]
    public class TestDownloadSettingsValidatorProvider
    {
        [SetUp]
        public void SetUp()
        {
            directoryServiceMock = Substitute.For<IDirectoryService>();

            systemUnderTest = NewDownloadSettingsValidatorProvider(directoryServiceMock);
        }

        private static readonly string[] BadDownloadUriValues = { null };

        private static readonly int[] BadMinimumWaitTimeInHoursValues = { 0, 101 };

        private static readonly int[] BadTimeoutValues = { -1, -2, 0, 99, 3601 };

        private static readonly string[] GoodDownloadUriValues = { "http://localhost/", @"C://file.txt" };

        private static readonly int[] GoodMinimumWaitTimeInHoursValues = { 1, 100 };

        private static readonly int[] GoodTimeoutValues = { 100, 3600 };

        private IDirectoryService directoryServiceMock;

        private IDownloadSettingsValidatorService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewDownloadSettingsValidatorProvider(null));
        }

        [Test]
        public void IsValidDownloadDirectory_WhenDirectoryDoesNotExist_ReturnsFalse()
        {
            directoryServiceMock.Exists("DownloadDirectory").Returns(false);

            bool actual = systemUnderTest.IsValidDownloadDirectory("DownloadDirectory");

            Assert.That(actual, Is.False);
        }

        [Test]
        public void IsValidDownloadDirectory_WhenDirectoryExists_ReturnsTrue()
        {
            directoryServiceMock.Exists("DownloadDirectory").Returns(true);

            bool actual = systemUnderTest.IsValidDownloadDirectory("DownloadDirectory");

            Assert.That(actual, Is.True);
        }

        [TestCase(100, 100)]
        public void TryParseMinimumWaitTimeSpan_WhenGoodValueIsProvided_ReturnsParsedTrue(int input, int hours)
        {
            var expected = new TimeSpan(hours, 0, 0);

            TimeSpan actual;
            systemUnderTest.TryParseMinimumWaitTimeSpan(input, out actual);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [TestCase(100, 100)]
        public void TryParseTimeout_WhenAnIntIsProvided_ReturnsTheParsedInt(int input, int expected)
        {
            int actual;
            systemUnderTest.TryParseTimeout(input, out actual);

            Assert.That(actual, Is.EqualTo(expected));
        }

        private IDownloadSettingsValidatorService NewDownloadSettingsValidatorProvider(
            IDirectoryService directoryService)
        {
            return new DownloadSettingsValidatorProvider(directoryService);
        }

        [TestCaseSource(nameof(BadDownloadUriValues))]
        public void TryParseDownloadUri_WhenBadValueIsProvided_ReturnsFalse(string input)
        {
            Uri downloadUri;
            bool actual = systemUnderTest.TryParseDownloadUri(input, out downloadUri);

            Assert.That(actual, Is.False);
        }

        [TestCaseSource(nameof(GoodDownloadUriValues))]
        public void TryParseDownloadUri_WhenGoodValueIsProvided_ReturnsParsedUri(string input)
        {
            Uri actual;
            systemUnderTest.TryParseDownloadUri(input, out actual);

            Assert.That(actual.OriginalString, Is.EqualTo(input));
        }

        [TestCaseSource(nameof(GoodDownloadUriValues))]
        public void TryParseDownloadUri_WhenGoodValueIsProvided_ReturnsTrue(string input)
        {
            Uri downloadUri;
            bool actual = systemUnderTest.TryParseDownloadUri(input, out downloadUri);

            Assert.That(actual, Is.True);
        }

        [TestCaseSource(nameof(BadMinimumWaitTimeInHoursValues))]
        public void TryParseMinimumWaitTimeSpan_WhenBadValueIsProvided_ReturnsFalse(int input)
        {
            TimeSpan output;
            bool actual = systemUnderTest.TryParseMinimumWaitTimeSpan(input, out output);

            Assert.That(actual, Is.False);
        }

        [TestCaseSource(nameof(BadMinimumWaitTimeInHoursValues))]
        public void TryParseMinimumWaitTimeSpan_WhenBadValueIsProvided_ReturnsParsedFalse(int input)
        {
            TimeSpan actual;
            systemUnderTest.TryParseMinimumWaitTimeSpan(input, out actual);

            Assert.That(actual, Is.EqualTo(TimeSpan.Zero));
        }

        [TestCaseSource(nameof(GoodMinimumWaitTimeInHoursValues))]
        public void TryParseMinimumWaitTimeSpan_WhenGoodValueIsProvided_ReturnsTrue(int input)
        {
            TimeSpan output;
            bool actual = systemUnderTest.TryParseMinimumWaitTimeSpan(input, out output);

            Assert.That(actual, Is.True);
        }

        [TestCaseSource(nameof(BadTimeoutValues))]
        public void TryParseTimeout_WhenAnIntNotInRangeIsProvided_ReturnsFalse(int input)
        {
            int output;
            bool actual = systemUnderTest.TryParseTimeout(input, out output);

            Assert.That(actual, Is.False);
        }

        [TestCaseSource(nameof(GoodTimeoutValues))]
        public void TryParseTimeout_WhenAnIntWithingRangeIsProvided_ReturnsTrue(int input)
        {
            int output;
            bool actual = systemUnderTest.TryParseTimeout(input, out output);

            Assert.That(actual, Is.True);
        }
    }
}