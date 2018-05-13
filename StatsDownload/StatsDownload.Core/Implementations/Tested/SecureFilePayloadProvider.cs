namespace StatsDownload.Core
{
    using System;
    using Interfaces.DataTransfer;
    using StatsDownload.Logging;

    public class SecureFilePayloadProvider : ISecureFilePayloadService
    {
        private readonly ILoggingService loggingService;

        public SecureFilePayloadProvider(ILoggingService loggingService)
        {
            if (loggingService == null)
            {
                throw new ArgumentNullException(nameof(loggingService));
            }

            this.loggingService = loggingService;
        }

        public void DisableSecureFilePayload(FilePayload filePayload)
        {
            UpdateDownloadUri(filePayload, Uri.UriSchemeHttps, Uri.UriSchemeHttp);
        }

        public void EnableSecureFilePayload(FilePayload filePayload)
        {
            UpdateDownloadUri(filePayload, Uri.UriSchemeHttp, Uri.UriSchemeHttps);
        }

        public bool IsSecureConnection(FilePayload filePayload)
        {
            return filePayload.DownloadUri.Scheme != Uri.UriSchemeHttp;
        }

        private void UpdateDownloadUri(FilePayload filePayload, string oldScheme, string newScheme)
        {
            Uri uri = filePayload.DownloadUri;

            if (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
            {
                if (uri.Scheme == oldScheme)
                {
                    var newUri = new Uri(uri.AbsoluteUri.Replace(oldScheme, newScheme));
                    loggingService.LogVerbose(
                        $"Changing scheme {oldScheme} to {newScheme}{Environment.NewLine}Old Uri: {uri.AbsoluteUri}{Environment.NewLine}New Uri: {newUri.AbsoluteUri}");
                    filePayload.DownloadUri = newUri;
                }
            }
        }
    }
}