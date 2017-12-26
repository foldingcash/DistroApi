namespace StatsDownload.Core
{
    public interface IFileReaderService
    {
        void ReadFile(StatsPayload statsPayload);
    }
}