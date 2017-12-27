namespace StatsDownload.FileServer.TestHarness
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Description;

    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var baseAddress = "http://127.0.0.1";
                var host = new ServiceHost(typeof(TestHarnessFileServer), new Uri(baseAddress));
                host.AddServiceEndpoint(typeof(ITestHarnessFileServer), new WebHttpBinding(), "")
                    .Behaviors.Add(new WebHttpBehavior());
                var smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                host.Description.Behaviors.Add(smb);
                host.Open();
                Console.WriteLine("Press {enter} to close server.");
                Console.ReadLine();
                host.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(
                    $"An exception was thrown. Details:{Environment.NewLine}{Environment.NewLine}{exception}{Environment.NewLine}");
                Console.WriteLine("Press {enter} to close server.");
                Console.ReadLine();
            }
        }
    }
}