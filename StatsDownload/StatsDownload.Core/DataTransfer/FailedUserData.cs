namespace StatsDownload.Core
{
    public class FailedUserData
    {
        public FailedUserData()
        {
        }

        public FailedUserData(string data)
        {
            Data = data;
        }

        public FailedUserData(string data, UserData userdata)
        {
            Data = data;
            UserData = userdata;
        }

        public string Data { get; private set; }

        public UserData UserData { get; private set; }
    }
}