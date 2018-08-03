namespace StatsDownloadApi.Interfaces
{
    public class ApiError
    {
        public ApiError(ApiErrorCode errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public ApiErrorCode ErrorCode { get; }

        public string ErrorMessage { get; }
    }
}