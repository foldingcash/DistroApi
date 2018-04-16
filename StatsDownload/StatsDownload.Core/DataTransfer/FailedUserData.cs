namespace StatsDownload.Core
{
    public class FailedUserData
    {
        public FailedUserData()
        {
        }

        public FailedUserData(int downloadId, int lineNumber, string data)
            : this(downloadId, lineNumber, data, null)
        {
        }

        public FailedUserData(int downloadId, int lineNumber, string data, UserData userdata)
        {
            DownloadId = downloadId;
            LineNumber = lineNumber;
            Data = data;
            UserData = userdata;
        }

        public string Data { get; private set; }

        public int DownloadId { get; private set; }

        public int LineNumber { get; private set; }

        public UserData UserData { get; private set; }
    }
}