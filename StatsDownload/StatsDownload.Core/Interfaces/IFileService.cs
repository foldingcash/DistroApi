namespace StatsDownload.Core.Interfaces
{
    using System.IO;

    public interface IFileService
    {
        void CreateFromStream(string path, Stream source);

        void Delete(string path);

        bool Exists(string path);

        void Move(string sourcePath, string destinationPath);
    }
}