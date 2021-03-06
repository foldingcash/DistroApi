﻿namespace StatsDownload.Core.Implementations
{
    using System;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;

    public class SecureFilePayloadProvider : ISecureFilePayloadService
    {
        private readonly ILogger logger;

        public SecureFilePayloadProvider(ILogger<SecureFilePayloadProvider> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                    logger.LogDebug(
                        $"Changing scheme {oldScheme} to {newScheme}{Environment.NewLine}Old Uri: {uri.AbsoluteUri}{Environment.NewLine}New Uri: {newUri.AbsoluteUri}");
                    filePayload.DownloadUri = newUri;
                }
            }
        }
    }
}