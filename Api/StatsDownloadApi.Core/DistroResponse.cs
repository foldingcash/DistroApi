namespace StatsDownloadApi.Core
{
    public class DistroResponse
    {
        public DistroResponse(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
}