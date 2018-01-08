namespace StatsDownload.SharpZipLib
{
    using System.IO;

    using ICSharpCode.SharpZipLib.BZip2;

    using StatsDownload.Core;

    public class Bz2CompressionProvider : IFileCompressionService
    {
        public void DecompressFile(FilePayload filePayload)
        {
            using (var sourceFile = new FileStream(filePayload.DownloadFilePath, FileMode.Open))
            {
                using (FileStream targetFile = File.Create(filePayload.UncompressedDownloadFilePath))
                {
                    BZip2.Decompress(sourceFile, targetFile, true);
                }
            }
        }
    }
}