namespace StatsDownload.SharpZipLib
{
    using System.IO;
    using Core.Interfaces.DataTransfer;
    using ICSharpCode.SharpZipLib.BZip2;

    using StatsDownload.Core.Exceptions;
    using StatsDownload.Core.Interfaces;

    public class Bz2CompressionProvider : IFileCompressionService
    {
        public void DecompressFile(FilePayload filePayload)
        {
            try
            {
                using (var sourceFile = new FileStream(filePayload.DownloadFilePath, FileMode.Open))
                {
                    using (FileStream targetFile = File.Create(filePayload.DecompressedDownloadFilePath))
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