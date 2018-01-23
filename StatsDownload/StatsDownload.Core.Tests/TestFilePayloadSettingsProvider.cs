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

        private IDownloadSettingsService downloadSettingsServiceMock;

        private IDownloadSettingsValidatorService downloadSettingsValidatorServiceMock;

        private IFilePayloadSettingsService systemUnderTest;

        private TimeSpan timeSpan;

        private Uri uri;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                () =>
                NewFilePayloadSettingsProvider(null, downloadSettingsServiceMock, downloadSettingsValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFilePayloadSettingsProvider(dateTimeServiceMock, null, downloadSettingsValidatorServiceMock));
            Assert.Throws<ArgumentNullException>(
                () => NewFilePayloadSettingsProvider(dateTimeServiceMock, downloadSettingsServiceMock, null));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenDownloadDirectoryDoesNotExist_ThrowsFileDownloadArgumentException(
            )
        {
            downloadSettingsValidatorServiceMock.IsValidDownloadDirectory("DownloadDirectory").Returns(false);

            var filePayload = new FilePayload();

            var actual =
                Assert.Throws<FileDownloadArgumentException>(() => InvokeSetFilePayloadDownloadDetails(filePayload));
            Assert.That(actual.Message, Is.EqualTo("Download directory is invalid"));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvalidAcceptAnySslCert_ReturnsDefault()
        {
            bool acceptAnySslCert;
            downloadSettingsValidatorServiceMock.TryParseAcceptAnySslCert("AcceptAnySslCert", out acceptAnySslCert)
                                                .Returns(callInfo => false);

            var filePayload = new FilePayload();

            InvokeSetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.AcceptAnySslCert, Is.False);
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvalidDownloadTimeoutSeconds_ReturnsDefault()
        {
            int timeout;
            downloadSettingsValidatorServiceMock.TryParseTimeout("DownloadTimeoutSeconds", out timeout)
                                                .Returns(callInfo => false);

            var filePayload = new FilePayload();

            InvokeSetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.TimeoutSeconds, Is.EqualTo(100));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvalidDownloadUri_ThrowsFileDownloadArgumentException()
        {
            Uri downloadUri;
            downloadSettingsValidatorServiceMock.TryParseDownloadUri("DownloadUri", out downloadUri)
                                                .Returns(callInfo => false);

            var filePayload = new FilePayload();

            var actual =
                Assert.Throws<FileDownloadArgumentException>(() => InvokeSetFilePayloadDownloadDetails(filePayload));
            Assert.That(actual.Message, Is.EqualTo("Download Uri is invalid"));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvalidMinimumWaitTimeInHours_ReturnsDefault()
        {
            TimeSpan minimumWaitTimeSpan;
            downloadSettingsValidatorServiceMock.TryParseMinimumWaitTimeSpan(
                "MinimumWaitTimeInHours",
                out minimumWaitTimeSpan).Returns(callInfo => false);

            var filePayload = new FilePayload();

            InvokeSetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.MinimumWaitTimeSpan, Is.EqualTo(MinimumWait.TimeSpan));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_DecompressedDownloadFileDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.DecompressedDownloadDirectory, Is.EqualTo("DownloadDirectory"));
            Assert.That(
                filePayload.DecompressedDownloadFileName,
                Is.EqualTo($"{dateTime.ToFileTime()}.daily_user_summary"));
            Assert.That(filePayload.DecompressedDownloadFileExtension, Is.EqualTo(".txt"));
            Assert.That(
                filePayload.DecompressedDownloadFilePath,
                Is.EqualTo($"DownloadDirectory\\{dateTime.ToFileTime()}.daily_user_summary.txt"));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_DownloadDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.DownloadUri.AbsoluteUri, Is.EqualTo("http://localhost/"));
            Assert.That(filePayload.TimeoutSeconds, Is.EqualTo(123));
            Assert.That(filePayload.AcceptAnySslCert, Is.EqualTo(true));
            Assert.That(filePayload.MinimumWaitTimeSpan, Is.EqualTo(timeSpan));
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
        public void SetFilePayloadDownloadDetails_WhenInvoked_FailedDownloadFileDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(
                filePayload.FailedDownloadFilePath,
                Is.EqualTo($"DownloadDirectory\\FileDownloadFailed\\{dateTime.ToFileTime()}.daily_user_summary.txt.bz2"));
        }

        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.Now;
            timeSpan = TimeSpan.MaxValue;
            uri = new Uri("http://localhost");

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(dateTime);

            downloadSettingsServiceMock = Substitute.For<IDownloadSettingsService>();
            downloadSettingsServiceMock.GetDownloadUri().Returns("DownloadUri");
            downloadSettingsServiceMock.GetDownloadTimeout().Returns("DownloadTimeoutSeconds");
            downloadSettingsServiceMock.GetDownloadDirectory().Returns("DownloadDirectory");
            downloadSettingsServiceMock.GetAcceptAnySslCert().Returns("AcceptAnySslCert");
            downloadSettingsServiceMock.GetMinimumWaitTimeInHours().Returns("MinimumWaitTimeInHours");

            downloadSettingsValidatorServiceMock = Substitute.For<IDownloadSettingsValidatorService>();

            int timeout;
            downloadSettingsValidatorServiceMock.TryParseTimeout("DownloadTimeoutSeconds", out timeout)
                                                .Returns(
                                                    callInfo =>
                                                    {
                                                        callInfo[1] = 123;
                                                        return true;
                                                    });
            bool acceptAnySslCert;
            downloadSettingsValidatorServiceMock.TryParseAcceptAnySslCert("AcceptAnySslCert", out acceptAnySslCert)
                                                .Returns(
                                                    callInfo =>
                                                    {
                                                        callInfo[1] = true;
                                                        return true;
                                                    });
            TimeSpan minimumWaitTimeSpan;
            downloadSettingsValidatorServiceMock.TryParseMinimumWaitTimeSpan(
                "MinimumWaitTimeInHours",
                out minimumWaitTimeSpan).Returns(
                    callInfo =>
                    {
                        callInfo[1] = timeSpan;
                        return true;
                    });
            Uri downloadUri;
            downloadSettingsValidatorServiceMock.TryParseDownloadUri("DownloadUri", out downloadUri).Returns(
                callInfo =>
                {
                    callInfo[1] = uri;
                    return true;
                });
            downloadSettingsValidatorServiceMock.IsValidDownloadDirectory("DownloadDirectory").Returns(true);

            systemUnderTest = NewFilePayloadSettingsProvider(
                dateTimeServiceMock,
                downloadSettingsServiceMock,
                downloadSettingsValidatorServiceMock);
        }

        private void InvokeSetFilePayloadDownloadDetails(FilePayload filePayload)
        {
            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);
        }

        private IFilePayloadSettingsService NewFilePayloadSettingsProvider(
            IDateTimeService dateTimeService,
            IDownloadSettingsService downloadSettingsService,
            IDownloadSettingsValidatorService downloadSettingsValidatorService)
        {
            return new FilePayloadSettingsProvider(
                dateTimeService,
                downloadSettingsService,
                downloadSettingsValidatorService);
        }
    }
}