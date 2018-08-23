namespace StatsDownload.StatsUpload.Console
{
    using System;
    using CastleWindsor;
    using Core.Interfaces;
    using NLog;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DependencyRegistration.Register();
                var service = WindsorContainer.Instance.Resolve<IStatsUploadService>();
                service.UploadStatsFiles();
            }
            catch (Exception ex)
            {
                TryLogException(ex);
            }
            finally
            {
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
            }
            finally
            {
                WindsorContainer.Instance.Release(logger);
            }
        }
    }
}