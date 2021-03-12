namespace StatsDownload.SharpZipLib
{
    using System;
    using System.IO;

    using ICSharpCode.SharpZipLib.BZip2;

    using Microsoft.Extensions.Logging;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.Exceptions;
    using StatsDownload.Logging;

    public class Bz2CompressionProvider : IFileCompressionService
    {
        private readonly ILogger<Bz2CompressionProvider> logger;

        public Bz2CompressionProvider(ILogger<Bz2CompressionProvider> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void CompressFile(string filePath, string compressedFilePath)
        {
            logger.LogMethodInvoked();
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
            finally
            {
                logger.LogMethodFinished();
            }
        }

        public void DecompressFile(string downloadFilePath, string decompressedDownloadFilePath)
        {
            logger.LogMethodInvoked();
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
            finally
            {
                logger.LogMethodFinished();
            }
        }
    }
}