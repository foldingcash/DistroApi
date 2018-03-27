namespace StatsDownload.StatsUpload.Console
{
    using StatsDownload.Core;

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
            finally
            {
                WindsorContainer.Dispose();
            }
        }
    }
}