namespace StatsDownload.FileDownload.Console
{
    using StatsDownload.Core;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                DependencyRegistration.Register();

                var service = WindsorContainer.Instance.Resolve<IFileDownloadService>();
                service.DownloadStatsFile();
            }
            finally
            {
                WindsorContainer.Dispose();
            }
        }
    }
}