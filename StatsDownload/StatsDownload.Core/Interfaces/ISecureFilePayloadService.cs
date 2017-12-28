namespace StatsDownload.Core
{
    public interface ISecureFilePayloadService
    {
        void DisableSecureFilePayload(FilePayload filePayload);

        void EnableSecureFilePayload(FilePayload filePayload);
    }
}