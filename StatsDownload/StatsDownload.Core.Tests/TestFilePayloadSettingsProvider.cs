namespace StatsDownload.Core.Tests
{
    using System;

    using NSubstitute;

    using NUnit.Framework;

    [TestFixture]
    public class TestFilePayloadSettingsProvider
    {
        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private IFileDownloadSettingsService fileDownloadSettingsServiceMock;

        private IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorServiceMock;

        private IFilePayloadSettingsService systemUnderTest;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFilePayloadSettingsProvider(
                    null,
                    fileDownloadSettingsServiceMock,
                    fileDownloadTimeoutValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFilePayloadSettingsProvider(dateTimeServiceMock, null, fileDownloadTimeoutValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFilePayloadSettingsProvider(dateTimeServiceMock, fileDownloadSettingsServiceMock, null));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_DownloadDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.DownloadUri.AbsoluteUri, Is.EqualTo("http://localhost/"));
            Assert.That(filePayload.TimeoutSeconds, Is.EqualTo(123));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_DownloadFileDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.DownloadDirectory, Is.EqualTo("DownloadDirectory"));
            Assert.That(filePayload.DownloadFileName, Is.EqualTo($"{dateTime.ToFileTime()}.daily_user_summary.txt"));
            Assert.That(filePayload.DownloadFileExtension, Is.EqualTo(".bz2"));
            Assert.That(
                filePayload.DownloadFilePath,
                Is.EqualTo($"DownloadDirectory\\{dateTime.ToFileTime()}.daily_user_summary.txt.bz2"));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_UncompressedDownloadFileDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.UncompressedDownloadDirectory, Is.EqualTo("DownloadDirectory"));
            Assert.That(
                filePayload.UncompressedDownloadFileName,
                Is.EqualTo($"{dateTime.ToFileTime()}.daily_user_summary"));
            Assert.That(filePayload.UncompressedDownloadFileExtension, Is.EqualTo(".txt"));
            Assert.That(
                filePayload.UncompressedDownloadFilePath,
                Is.EqualTo($"DownloadDirectory\\{dateTime.ToFileTime()}.daily_user_summary.txt"));
        }

        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.Now;

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(dateTime);

            fileDownloadSettingsServiceMock = Substitute.For<IFileDownloadSettingsService>();
            fileDownloadSettingsServiceMock.GetDownloadUri().Returns("http://localhost");
            fileDownloadSettingsServiceMock.GetDownloadTimeout().Returns("DownloadTimeoutSeconds");
            fileDownloadSettingsServiceMock.GetDownloadDirectory().Returns("DownloadDirectory");

            int timeout;
            fileDownloadTimeoutValidatorServiceMock = Substitute.For<IFileDownloadTimeoutValidatorService>();
            fileDownloadTimeoutValidatorServiceMock.TryParseTimeout("DownloadTimeoutSeconds", out timeout)
                .Returns(
                    callInfo =>
                        {
                            callInfo[1] = 123;
                            return true;
                        });

            systemUnderTest = NewFilePayloadSettingsProvider(
                dateTimeServiceMock,
                fileDownloadSettingsServiceMock,
                fileDownloadTimeoutValidatorServiceMock);
        }

        private IFilePayloadSettingsService NewFilePayloadSettingsProvider(
            IDateTimeService dateTimeService,
            IFileDownloadSettingsService fileDownloadSettingsService,
            IFileDownloadTimeoutValidatorService fileDownloadTimeoutValidatorService)
        {
            return new FilePayloadSettingsProvider(
                dateTimeService,
                fileDownloadSettingsService,
                fileDownloadTimeoutValidatorService);
        }
    }
}