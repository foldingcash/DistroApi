namespace StatsDownload.Parsing.Tests
{
    using System;

    using Microsoft.Extensions.Options;

    using NSubstitute;

    using NUnit.Framework;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.Core.Interfaces.Exceptions;
    using StatsDownload.Core.Interfaces.Logging;
    using StatsDownload.Core.Interfaces.Settings;

    [TestFixture]
    public class TestFilePayloadSettingsProvider
    {
        [SetUp]
        public void SetUp()
        {
            dateTime = DateTime.UtcNow;
            timeSpan = TimeSpan.MaxValue;
            uri = new Uri("http://localhost");

            dateTimeServiceMock = Substitute.For<IDateTimeService>();
            dateTimeServiceMock.DateTimeNow().Returns(dateTime);

            downloadSettings = new DownloadSettings
                               {
                                   DownloadUri = "DownloadUri",
                                   DownloadTimeout = 100,
                                   DownloadDirectory = "DownloadDirectory",
                                   AcceptAnySslCert = true,
                                   MinimumWaitTimeInHours = 200
                               };
            downloadSettingsOptionsMock = Substitute.For<IOptions<DownloadSettings>>();
            downloadSettingsOptionsMock.Value.Returns(downloadSettings);

            downloadSettingsValidatorServiceMock = Substitute.For<IDownloadSettingsValidatorService>();

            int timeout;
            downloadSettingsValidatorServiceMock.TryParseTimeout(100, out timeout).Returns(callInfo =>
            {
                callInfo[1] = 123;
                return true;
            });
            TimeSpan minimumWaitTimeSpan;
            downloadSettingsValidatorServiceMock.TryParseMinimumWaitTimeSpan(200, out minimumWaitTimeSpan).Returns(
                callInfo =>
                {
                    callInfo[1] = timeSpan;
                    return true;
                });
            Uri downloadUri;
            downloadSettingsValidatorServiceMock.TryParseDownloadUri("DownloadUri", out downloadUri).Returns(callInfo =>
            {
                callInfo[1] = uri;
                return true;
            });
            downloadSettingsValidatorServiceMock.IsValidDownloadDirectory("DownloadDirectory").Returns(true);

            loggingServiceMock = Substitute.For<ILoggingService>();

            dataStoreSettings = new DataStoreSettings { UploadDirectory = "UploadDirectory" };

            dataStoreSettingsOptionsMock = Substitute.For<IOptions<DataStoreSettings>>();
            dataStoreSettingsOptionsMock.Value.Returns(dataStoreSettings);

            guidServiceMock = Substitute.For<IGuidService>();
            guidServiceMock.NewGuid().Returns(guid);

            systemUnderTest = NewFilePayloadSettingsProvider(dateTimeServiceMock, downloadSettingsOptionsMock,
                downloadSettingsValidatorServiceMock, loggingServiceMock, dataStoreSettingsOptionsMock,
                guidServiceMock);
        }

        private DataStoreSettings dataStoreSettings;

        private IOptions<DataStoreSettings> dataStoreSettingsOptionsMock;

        private DateTime dateTime;

        private IDateTimeService dateTimeServiceMock;

        private DownloadSettings downloadSettings;

        private IOptions<DownloadSettings> downloadSettingsOptionsMock;

        private IDownloadSettingsValidatorService downloadSettingsValidatorServiceMock;

        private readonly Guid guid = Guid.NewGuid();

        private IGuidService guidServiceMock;

        private ILoggingService loggingServiceMock;

        private IFilePayloadSettingsService systemUnderTest;

        private TimeSpan timeSpan;

        private Uri uri;

        [Test]
        public void Constructor_WhenNullDependencyProvided_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadSettingsProvider(null, downloadSettingsOptionsMock,
                downloadSettingsValidatorServiceMock, loggingServiceMock, dataStoreSettingsOptionsMock,
                guidServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadSettingsProvider(dateTimeServiceMock, null,
                downloadSettingsValidatorServiceMock, loggingServiceMock, dataStoreSettingsOptionsMock,
                guidServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadSettingsProvider(dateTimeServiceMock,
                downloadSettingsOptionsMock, null, loggingServiceMock, dataStoreSettingsOptionsMock, guidServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadSettingsProvider(dateTimeServiceMock,
                downloadSettingsOptionsMock, downloadSettingsValidatorServiceMock, null, dataStoreSettingsOptionsMock,
                guidServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadSettingsProvider(dateTimeServiceMock,
                downloadSettingsOptionsMock, downloadSettingsValidatorServiceMock, loggingServiceMock, null,
                guidServiceMock));
            Assert.Throws<ArgumentNullException>(() => NewFilePayloadSettingsProvider(dateTimeServiceMock,
                downloadSettingsOptionsMock, downloadSettingsValidatorServiceMock, loggingServiceMock,
                dataStoreSettingsOptionsMock, null));
        }

        [Test]
        public void
            SetFilePayloadDownloadDetails_WhenDownloadDirectoryDoesNotExist_ThrowsFileDownloadArgumentException()
        {
            downloadSettingsValidatorServiceMock.IsValidDownloadDirectory("DownloadDirectory").Returns(false);

            var filePayload = new FilePayload();

            var actual =
                Assert.Throws<FileDownloadArgumentException>(() => InvokeSetFilePayloadDownloadDetails(filePayload));
            Assert.That(actual.Message, Is.EqualTo("Download directory is invalid"));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvalidDownloadTimeoutSeconds_ReturnsDefault()
        {
            int timeout;
            downloadSettingsValidatorServiceMock.TryParseTimeout(100, out timeout).Returns(callInfo => false);

            var filePayload = new FilePayload();

            InvokeSetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.TimeoutSeconds, Is.EqualTo(100));
            loggingServiceMock.Received()
                              .LogDebug("The download timeout configuration was invalid, using the default value.");
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
            downloadSettingsValidatorServiceMock.TryParseMinimumWaitTimeSpan(200, out minimumWaitTimeSpan)
                                                .Returns(callInfo => false);

            var filePayload = new FilePayload();

            InvokeSetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.MinimumWaitTimeSpan, Is.EqualTo(MinimumWait.TimeSpan));
            loggingServiceMock.Received()
                              .LogDebug("The minimum wait time configuration was invalid, using the default value.");
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_DecompressedDownloadFileDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.DecompressedDownloadDirectory, Is.EqualTo("DownloadDirectory"));
            Assert.That(filePayload.DecompressedDownloadFileName,
                Is.EqualTo($"{dateTime.ToFileTime()}.{guid}.daily_user_summary"));
            Assert.That(filePayload.DecompressedDownloadFileExtension, Is.EqualTo("txt"));
            Assert.That(filePayload.DecompressedDownloadFilePath,
                Is.EqualTo($"DownloadDirectory\\{dateTime.ToFileTime()}.{guid}.daily_user_summary.txt"));
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
            Assert.That(filePayload.DownloadFileName,
                Is.EqualTo($"{dateTime.ToFileTime()}.{guid}.daily_user_summary.txt"));
            Assert.That(filePayload.DownloadFileExtension, Is.EqualTo("bz2"));
            Assert.That(filePayload.DownloadFilePath,
                Is.EqualTo($"DownloadDirectory\\{dateTime.ToFileTime()}.{guid}.daily_user_summary.txt.bz2"));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_FailedDownloadFileDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.FailedDownloadFilePath,
                Is.EqualTo(
                    $"DownloadDirectory\\FileDownloadFailed\\{dateTime.ToFileTime()}.{guid}.daily_user_summary.txt.bz2"));
        }

        [Test]
        public void SetFilePayloadDownloadDetails_WhenInvoked_UploadDetailsAreSet()
        {
            var filePayload = new FilePayload();

            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);

            Assert.That(filePayload.UploadPath,
                Is.EqualTo($"UploadDirectory\\{dateTime.ToFileTime()}.{guid}.daily_user_summary.txt.bz2"));
        }

        private void InvokeSetFilePayloadDownloadDetails(FilePayload filePayload)
        {
            systemUnderTest.SetFilePayloadDownloadDetails(filePayload);
        }

        private IFilePayloadSettingsService NewFilePayloadSettingsProvider(
            IDateTimeService dateTimeService, IOptions<DownloadSettings> downloadSettingsOptions,
            IDownloadSettingsValidatorService downloadSettingsValidatorService, ILoggingService loggingService,
            IOptions<DataStoreSettings> dataStoreSettingsOptions, IGuidService guidService)
        {
            return new FilePayloadSettingsProvider(dateTimeService, downloadSettingsOptions,
                downloadSettingsValidatorService, loggingService, dataStoreSettingsOptions, guidService);
        }
    }
}