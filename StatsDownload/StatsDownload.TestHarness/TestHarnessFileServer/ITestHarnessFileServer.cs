namespace StatsDownload.TestHarness
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface ITestHarnessFileServer
    {
        [OperationContract]
        [WebGet(UriTemplate = "/daily_user_summary.txt.bz2")]
        Stream GetFile();
    }
}