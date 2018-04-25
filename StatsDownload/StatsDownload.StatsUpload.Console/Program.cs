namespace StatsDownload.StatsUpload.Console
{
    using System;

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                WindsorContainer.Dispose();
            }
        }
    }
}