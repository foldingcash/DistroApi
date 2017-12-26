namespace StatsDownload.SharpZipLib
{
    using System.IO;

    using ICSharpCode.SharpZipLib.BZip2;

    using StatsDownload.Core;

    public class Bz2CompressionProvider : IFileCompressionService
    {
        public void DecompressFile(StatsPayload statsPayload)
        {
            using (var sourceFile = new FileStream(statsPayload.DownloadFilePath, FileMode.Open))
            {
                using (FileStream targetFile = File.Create(statsPayload.UncompressedDownloadFilePath))
                {
                    BZip2.Decompress(sourceFile, targetFile, true);
                }
            }
        }
    }
}