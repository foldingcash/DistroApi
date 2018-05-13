namespace StatsDownload.Core.Interfaces
{
    public interface IFileService
    {
        void Delete(string path);

        bool Exists(string path);

        void Move(string sourcePath, string destinationPath);
    }
}