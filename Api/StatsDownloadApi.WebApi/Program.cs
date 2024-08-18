namespace StatsDownloadApi.WebApi
{
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using NBitcoin;

    public class Program
    {
        public static void Main(string[] args)
        {
            var temp = new BitcoinPubKeyAddress("14WPq1y6Nd1QfMks17hNBTrbQsYoLswicz", Network.Main);
            var valid = temp.VerifyMessage("test", "HxWJlzbey8MAjdVH8qsopRgTX0m/4G47Gu8mw/Xwp4j6TcvwKgWzNCUQDuQV9LcdcsSy/y8BpDND6AxqpAiiARA=");

            BuildWebHost(args).Run();
        }

        private static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build();
        }
    }
}