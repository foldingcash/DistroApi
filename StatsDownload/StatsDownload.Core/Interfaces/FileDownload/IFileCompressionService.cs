namespace StatsDownload.Core
{
    public interface IFileCompressionService
    {
        void DecompressFile(StatsPayload statsPayload);
    }
}