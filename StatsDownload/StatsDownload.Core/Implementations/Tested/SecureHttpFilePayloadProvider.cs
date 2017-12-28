namespace StatsDownload.Core
{
    using System;

    public class SecureHttpFilePayloadProvider : ISecureFilePayloadService
    {
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
            Uri uri = filePayload.DownloadUri;

            if (uri.Scheme == Uri.UriSchemeHttps)
            {
                return true;
            }

            return false;
        }

        private void UpdateDownloadUri(FilePayload filePayload, string oldScheme, string newScheme)
        {
            Uri uri = filePayload.DownloadUri;

            if (uri.Scheme == oldScheme)
            {
                filePayload.DownloadUri = new Uri(uri.AbsoluteUri.Replace(oldScheme, newScheme));
            }
        }
    }
}