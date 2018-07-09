namespace StatsDownloadApi.Interfaces
{
    public class DistroError
    {
        public DistroError(DistroErrorCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public DistroErrorCode ErrorCode { get; set; }

        public string ErrorMessage { get; set; }
    }
}