namespace StatsDownload.FileDownload.Console
{
    using System;
    using System.Threading.Tasks;

    using NLog;

    using StatsDownload.Core.Interfaces;
    using StatsDownload.Core.Interfaces.DataTransfer;
    using StatsDownload.FileDownload.Console.CastleWindsor;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            IFileDownloadService service = null;

            try
            {
                DependencyRegistration.Register();
                service = WindsorContainer.Instance.Resolve<IFileDownloadService>();
                FileDownloadResult results = await service.DownloadStatsFile();
                Environment.Exit(results.Success ? 0 : -1);
            }
            catch (Exception ex)
            {
                TryLogException(ex);
                Environment.Exit(-1);
            }
            finally
            {
                WindsorContainer.Instance.Release(service);
                WindsorContainer.Dispose();
                LogManager.Shutdown();
            }
        }

        private static void TryLogException(Exception exception)
        {
            ILogger logger = null;

            try
            {
                logger = WindsorContainer.Instance.Resolve<ILogger>();
                logger.Error(exception);
            }
            catch (Exception ex)
            {
                // As a last resort log to standard output
                Console.WriteLine(ex.ToString());
                Console.WriteLine(exception.ToString());
            }
            finally
            {
                WindsorContainer.Instance.Release(logger);
            }
        }
    }
}