namespace StatsDownload.SharpZipLib
{
    using System.IO;

    using ICSharpCode.SharpZipLib.BZip2;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Exceptions;

    public class Bz2CompressionProvider : IFileCompressionService
    {
        public void CompressFile(string filePath, string compressedFilePath)
        {
            try
            {
                using (var sourceFile = new FileStream(filePath, FileMode.Open))
                {
                    using (FileStream targetFile = File.Create(compressedFilePath))
                    {
                        BZip2.Compress(sourceFile, targetFile, true, 5);
                    }
                }
            }
            catch (BZip2Exception bZip2Exception)
            {
                throw new FileDownloadFailedCompressionException("BZip2 Failed Decompression", bZip2Exception);
            }
        }

        public void DecompressFile(string downloadFilePath, string decompressedDownloadFilePath)
        {
            try
            {
                using (var sourceFile = new FileStream(downloadFilePath, FileMode.Open))
                {
                    using (FileStream targetFile = File.Create(decompressedDownloadFilePath))
                    {
                        BZip2.Decompress(sourceFile, targetFile, true);
                    }
                }
            }
            catch (BZip2Exception bZip2Exception)
            {
                throw new FileDownloadFailedDecompressionException("BZip2 Failed Decompression", bZip2Exception);
            }
        }
    }
}