namespace StatsDownload.FileServer.TestHarness
{
    using System.IO;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    [ServiceContract]
    public interface ITestHarnessFileServer
    {
        [OperationContract]
        [WebGet(UriTemplate = "/decompressable.bz2")]
        Stream GetDecompressableFile();

        [OperationContract]
        [WebGet(UriTemplate = "/fail_download.bz2")]
        Stream GetFailDownloadFile();

        [OperationContract]
        [WebGet(UriTemplate = "/daily_user_summary.txt.bz2")]
        Stream GetFile();

        [OperationContract]
        [WebGet(UriTemplate = "/timeout.bz2")]
        Stream GetTimeoutFile();
    }
}