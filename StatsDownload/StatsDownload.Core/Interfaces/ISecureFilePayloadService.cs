namespace StatsDownload.Core
{
    using Interfaces.DataTransfer;

    public interface ISecureFilePayloadService
    {
        void DisableSecureFilePayload(FilePayload filePayload);

        void EnableSecureFilePayload(FilePayload filePayload);

        bool IsSecureConnection(FilePayload filePayload);
    }
}