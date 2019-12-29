namespace StatsDownload.Core.Interfaces
{
    using System.Threading.Tasks;

    using StatsDownload.Core.Interfaces.DataTransfer;

    public interface IDataStoreService
    {
        Task DownloadFile(FilePayload filePayload, ValidatedFile validatedFile);

        Task<bool> IsAvailable();

        Task UploadFile(FilePayload filePayload);
    }
}